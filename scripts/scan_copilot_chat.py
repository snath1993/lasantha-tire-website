#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Scan VS Code Copilot Chat History
"""
import sqlite3
import json
import os
from pathlib import Path
from datetime import datetime

def scan_global_state():
    """Scan VS Code global state for Copilot chat history"""
    global_state_db = r'C:\Users\Cashi\AppData\Roaming\Code\User\globalStorage\state.vscdb'
    
    if not os.path.exists(global_state_db):
        print(f"Database not found: {global_state_db}")
        return
    
    print(f"Scanning: {global_state_db}")
    print(f"Size: {os.path.getsize(global_state_db) / 1024 / 1024:.2f} MB")
    print("-" * 80)
    
    conn = sqlite3.connect(global_state_db)
    cursor = conn.cursor()
    
    # Get tables
    cursor.execute("SELECT name FROM sqlite_master WHERE type='table'")
    tables = cursor.fetchall()
    print(f"Tables: {[t[0] for t in tables]}")
    
    # Find copilot/chat related keys
    cursor.execute("SELECT key FROM ItemTable WHERE key LIKE '%copilot%' OR key LIKE '%chat%'")
    keys = cursor.fetchall()
    
    print(f"\nFound {len(keys)} Copilot/Chat related keys:")
    print("-" * 80)
    
    chat_sessions = []
    
    for (key,) in keys:
        cursor.execute("SELECT value FROM ItemTable WHERE key = ?", (key,))
        result = cursor.fetchone()
        if result:
            value = result[0]
            size_kb = len(value) / 1024 if value else 0
            
            # Only show relevant large items
            if size_kb > 1 or 'session' in key.lower() or 'history' in key.lower():
                print(f"\n📌 Key: {key}")
                print(f"   Size: {size_kb:.2f} KB")
                
                # Try to parse JSON
                try:
                    data = json.loads(value)
                    if isinstance(data, dict):
                        print(f"   Type: dict with {len(data)} keys")
                        if len(data) < 10:
                            for k in list(data.keys())[:5]:
                                print(f"     - {k}")
                    elif isinstance(data, list):
                        print(f"   Type: list with {len(data)} items")
                        if 'session' in key.lower() or 'chat' in key.lower():
                            chat_sessions.append((key, data))
                except:
                    preview = value[:200] if isinstance(value, str) else str(value)[:200]
                    print(f"   Preview: {preview}...")
    
    print("\n" + "=" * 80)
    print("DETAILED CHAT SESSION ANALYSIS")
    print("=" * 80)
    
    # Find the main chat sessions storage
    cursor.execute("SELECT key, value FROM ItemTable WHERE key LIKE '%interactive%' OR key LIKE '%session%'")
    sessions = cursor.fetchall()
    
    for key, value in sessions:
        if value and len(value) > 1000:
            try:
                data = json.loads(value)
                print(f"\n📂 {key}")
                if isinstance(data, list):
                    print(f"   Sessions count: {len(data)}")
                    for i, session in enumerate(data[:5]):
                        if isinstance(session, dict):
                            print(f"   Session {i+1}: {list(session.keys())[:5]}")
            except:
                pass
    
    conn.close()
    return chat_sessions

def scan_workspace_storages():
    """Scan workspace-specific chat histories"""
    ws_path = Path(r'C:\Users\Cashi\AppData\Roaming\Code\User\workspaceStorage')
    
    print("\n" + "=" * 80)
    print("WORKSPACE-SPECIFIC CHAT HISTORIES")
    print("=" * 80)
    
    workspaces_with_chat = []
    
    for ws_dir in ws_path.iterdir():
        if not ws_dir.is_dir():
            continue
        
        state_db = ws_dir / 'state.vscdb'
        workspace_json = ws_dir / 'workspace.json'
        
        if not state_db.exists():
            continue
        
        db_size = state_db.stat().st_size
        if db_size < 50000:  # Skip small databases
            continue
        
        workspace_name = "Unknown"
        if workspace_json.exists():
            try:
                ws_data = json.loads(workspace_json.read_text())
                workspace_name = ws_data.get('folder', ws_data.get('workspace', 'Unknown'))
            except:
                pass
        
        # Check for copilot chat data
        try:
            conn = sqlite3.connect(str(state_db))
            cursor = conn.cursor()
            cursor.execute("SELECT COUNT(*) FROM ItemTable WHERE key LIKE '%copilot%' OR key LIKE '%chat%'")
            chat_count = cursor.fetchone()[0]
            
            if chat_count > 0:
                cursor.execute("SELECT key, LENGTH(value) as size FROM ItemTable WHERE key LIKE '%copilot%' OR key LIKE '%chat%' ORDER BY size DESC LIMIT 5")
                top_items = cursor.fetchall()
                
                workspaces_with_chat.append({
                    'dir': ws_dir.name,
                    'workspace': workspace_name,
                    'db_size_kb': db_size / 1024,
                    'chat_items': chat_count,
                    'top_items': top_items
                })
            
            conn.close()
        except Exception as e:
            pass
    
    # Sort by db size
    workspaces_with_chat.sort(key=lambda x: x['db_size_kb'], reverse=True)
    
    for ws in workspaces_with_chat[:15]:
        print(f"\n📁 Workspace: {ws['workspace']}")
        print(f"   Folder: {ws['dir']}")
        print(f"   DB Size: {ws['db_size_kb']:.1f} KB")
        print(f"   Chat Items: {ws['chat_items']}")
        if ws['top_items']:
            print("   Top Items:")
            for key, size in ws['top_items']:
                print(f"     - {key}: {size/1024:.1f} KB")

def extract_chat_messages():
    """Extract actual chat messages from Copilot chat history"""
    global_state_db = r'C:\Users\Cashi\AppData\Roaming\Code\User\globalStorage\state.vscdb'
    
    print("\n" + "=" * 80)
    print("EXTRACTING CHAT MESSAGES")
    print("=" * 80)
    
    conn = sqlite3.connect(global_state_db)
    cursor = conn.cursor()
    
    # Look for chat sessions
    cursor.execute("SELECT key, value FROM ItemTable WHERE (key LIKE '%copilot%' OR key LIKE '%interactive%' OR key LIKE '%chat%') AND LENGTH(value) > 5000")
    results = cursor.fetchall()
    
    total_messages = 0
    
    for key, value in results:
        try:
            data = json.loads(value)
            
            # Check different structures for chat messages
            if isinstance(data, list):
                for item in data:
                    if isinstance(item, dict):
                        # Check for messages array
                        if 'requests' in item:
                            requests = item['requests']
                            if isinstance(requests, list):
                                print(f"\n📝 Found {len(requests)} chat requests in {key[:50]}...")
                                total_messages += len(requests)
                                
                                for req in requests[:3]:  # Show first 3
                                    if isinstance(req, dict):
                                        msg = req.get('message', req.get('prompt', req.get('text', '')))
                                        if isinstance(msg, str):
                                            preview = msg[:150].replace('\n', ' ')
                                            print(f"   User: {preview}...")
                                        elif isinstance(msg, dict):
                                            text = msg.get('text', msg.get('content', ''))
                                            preview = str(text)[:150].replace('\n', ' ')
                                            print(f"   User: {preview}...")
        except Exception as e:
            pass
    
    print(f"\n✅ Total chat messages found: {total_messages}")
    conn.close()

if __name__ == '__main__':
    print("=" * 80)
    print("GITHUB COPILOT CHAT HISTORY SCANNER")
    print("=" * 80)
    print(f"Scan Time: {datetime.now()}")
    print()
    
    scan_global_state()
    scan_workspace_storages()
    extract_chat_messages()
    
    print("\n" + "=" * 80)
    print("SCAN COMPLETE")
    print("=" * 80)

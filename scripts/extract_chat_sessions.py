#!/usr/bin/env python
# -*- coding: utf-8 -*-
import sqlite3
import json
import os
from datetime import datetime

db = r'C:\Users\Cashi\AppData\Roaming\Code\User\globalStorage\state.vscdb'
conn = sqlite3.connect(db)
cursor = conn.cursor()

# Get the chat session store index
cursor.execute("SELECT value FROM ItemTable WHERE key = 'chat.ChatSessionStore.index'")
result = cursor.fetchone()
if result:
    data = json.loads(result[0])
    entries = data.get('entries', {})
    if isinstance(entries, dict):
        entries = list(entries.values())
    print(f'Total chat sessions in global store: {len(entries)}')
    print('=' * 80)
    
    for entry in list(entries)[:20]:
        session_id = entry.get('sessionId', 'unknown')
        print(f"\nSession: {session_id}")
        
        # Try to get the actual session data
        session_key = f'chat.ChatSessionStore.{session_id}'
        cursor.execute("SELECT value FROM ItemTable WHERE key = ?", (session_key,))
        session_result = cursor.fetchone()
        if session_result:
            session_data = json.loads(session_result[0])
            if isinstance(session_data, dict):
                title = session_data.get('customTitle', session_data.get('title', 'No title'))
                created = session_data.get('createdAt', 'Unknown')
                requests = session_data.get('requests', [])
                print(f"  Title: {title}")
                print(f"  Created: {created}")
                print(f"  Messages: {len(requests)}")
                
                # Show some messages
                for i, req in enumerate(requests[:3]):
                    if isinstance(req, dict):
                        msg = req.get('message', {})
                        if isinstance(msg, dict):
                            text = msg.get('text', '')[:200]
                        else:
                            text = str(msg)[:200]
                        print(f"    [{i+1}] {text.replace(chr(10), ' ')}")

print('\n' + '=' * 80)
print('Scanning workspace-specific sessions...')
print('=' * 80)

# Check workspace storage for whatsapp-sql-api
ws_db = r'C:\Users\Cashi\AppData\Roaming\Code\User\workspaceStorage\080c9b652a6a305ef346e499f39074e3\state.vscdb'
if os.path.exists(ws_db):
    conn2 = sqlite3.connect(ws_db)
    cursor2 = conn2.cursor()
    
    cursor2.execute("SELECT value FROM ItemTable WHERE key = 'chat.ChatSessionStore.index'")
    result = cursor2.fetchone()
    if result:
        data = json.loads(result[0])
        entries = data.get('entries', [])
        print(f'\nWhatsApp-SQL-API workspace sessions: {len(entries)}')
        
        for entry in entries[:10]:
            session_id = entry.get('sessionId', 'unknown')
            cursor2.execute("SELECT value FROM ItemTable WHERE key = ?", (f'chat.ChatSessionStore.{session_id}',))
            session_result = cursor2.fetchone()
            if session_result:
                session_data = json.loads(session_result[0])
                if isinstance(session_data, dict):
                    title = session_data.get('customTitle', session_data.get('title', 'No title'))
                    requests = session_data.get('requests', [])
                    print(f"\n  📝 {title}")
                    print(f"     Messages: {len(requests)}")
                    for i, req in enumerate(requests[:2]):
                        if isinstance(req, dict):
                            msg = req.get('message', {})
                            if isinstance(msg, dict):
                                text = msg.get('text', '')[:150]
                            else:
                                text = str(msg)[:150]
                            print(f"       User: {text.replace(chr(10), ' ')}")
    
    conn2.close()

conn.close()

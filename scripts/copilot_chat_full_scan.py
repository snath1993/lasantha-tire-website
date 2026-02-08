#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Complete Copilot Chat History Scanner - Sinhala Summary Version
"""
import sqlite3
import json
import os
from datetime import datetime
from collections import defaultdict

def get_entries_list(entries):
    """Convert entries to list regardless of format"""
    if isinstance(entries, dict):
        return list(entries.values())
    elif isinstance(entries, list):
        return entries
    return []

def scan_all_chat_history():
    """Scan all VS Code Copilot chat history"""
    
    results = {
        'global_sessions': [],
        'workspace_sessions': {},
        'total_messages': 0,
        'topics': defaultdict(int)
    }
    
    # Global state database
    global_db = r'C:\Users\Cashi\AppData\Roaming\Code\User\globalStorage\state.vscdb'
    
    if os.path.exists(global_db):
        conn = sqlite3.connect(global_db)
        cursor = conn.cursor()
        
        # Get session index
        cursor.execute("SELECT value FROM ItemTable WHERE key = 'chat.ChatSessionStore.index'")
        result = cursor.fetchone()
        
        if result:
            data = json.loads(result[0])
            entries = get_entries_list(data.get('entries', {}))
            
            for entry in entries:
                if isinstance(entry, dict):
                    session_id = entry.get('sessionId', '')
                elif isinstance(entry, str):
                    session_id = entry
                else:
                    continue
                
                # Get session details
                cursor.execute("SELECT value FROM ItemTable WHERE key = ?", (f'chat.ChatSessionStore.{session_id}',))
                session_result = cursor.fetchone()
                
                if session_result:
                    try:
                        session_data = json.loads(session_result[0])
                        if isinstance(session_data, dict):
                            requests = session_data.get('requests', [])
                            title = session_data.get('customTitle', '') or session_data.get('title', 'Untitled')
                            
                            session_info = {
                                'id': session_id,
                                'title': title,
                                'message_count': len(requests),
                                'messages': []
                            }
                            
                            results['total_messages'] += len(requests)
                            
                            for req in requests:
                                if isinstance(req, dict):
                                    msg = req.get('message', {})
                                    if isinstance(msg, dict):
                                        text = msg.get('text', '')
                                    else:
                                        text = str(msg)
                                    
                                    if text:
                                        session_info['messages'].append(text[:500])
                                        
                                        # Categorize by topic
                                        text_lower = text.lower()
                                        if 'whatsapp' in text_lower:
                                            results['topics']['WhatsApp'] += 1
                                        if 'invoice' in text_lower or 'bill' in text_lower:
                                            results['topics']['Invoice/Billing'] += 1
                                        if 'crystal' in text_lower or 'report' in text_lower:
                                            results['topics']['Crystal Reports'] += 1
                                        if 'database' in text_lower or 'sql' in text_lower:
                                            results['topics']['Database/SQL'] += 1
                                        if 'api' in text_lower:
                                            results['topics']['API'] += 1
                                        if 'dashboard' in text_lower:
                                            results['topics']['Dashboard'] += 1
                                        if 'wpf' in text_lower or 'desktop' in text_lower:
                                            results['topics']['WPF/Desktop'] += 1
                                        if 'peachtree' in text_lower:
                                            results['topics']['Peachtree'] += 1
                                        if 'error' in text_lower or 'fix' in text_lower or 'bug' in text_lower:
                                            results['topics']['Bug Fixes'] += 1
                            
                            results['global_sessions'].append(session_info)
                    except:
                        pass
        
        conn.close()
    
    # Scan workspace storages
    ws_path = r'C:\Users\Cashi\AppData\Roaming\Code\User\workspaceStorage'
    
    for ws_dir in os.listdir(ws_path):
        ws_full_path = os.path.join(ws_path, ws_dir)
        if not os.path.isdir(ws_full_path):
            continue
        
        state_db = os.path.join(ws_full_path, 'state.vscdb')
        workspace_json = os.path.join(ws_full_path, 'workspace.json')
        
        if not os.path.exists(state_db):
            continue
        
        workspace_name = "Unknown"
        if os.path.exists(workspace_json):
            try:
                with open(workspace_json, 'r') as f:
                    ws_data = json.load(f)
                    workspace_name = ws_data.get('folder', ws_data.get('workspace', 'Unknown'))
                    workspace_name = workspace_name.replace('file:///', '').replace('%3A', ':').replace('%20', ' ')
            except:
                pass
        
        try:
            conn = sqlite3.connect(state_db)
            cursor = conn.cursor()
            
            cursor.execute("SELECT value FROM ItemTable WHERE key = 'chat.ChatSessionStore.index'")
            result = cursor.fetchone()
            
            if result:
                data = json.loads(result[0])
                entries = get_entries_list(data.get('entries', {}))
                
                ws_sessions = []
                
                for entry in entries:
                    if isinstance(entry, dict):
                        session_id = entry.get('sessionId', '')
                    elif isinstance(entry, str):
                        session_id = entry
                    else:
                        continue
                    
                    cursor.execute("SELECT value FROM ItemTable WHERE key = ?", (f'chat.ChatSessionStore.{session_id}',))
                    session_result = cursor.fetchone()
                    
                    if session_result:
                        try:
                            session_data = json.loads(session_result[0])
                            if isinstance(session_data, dict):
                                requests = session_data.get('requests', [])
                                title = session_data.get('customTitle', '') or session_data.get('title', 'Untitled')
                                
                                session_info = {
                                    'title': title,
                                    'message_count': len(requests),
                                    'first_messages': []
                                }
                                
                                results['total_messages'] += len(requests)
                                
                                for req in requests[:5]:
                                    if isinstance(req, dict):
                                        msg = req.get('message', {})
                                        if isinstance(msg, dict):
                                            text = msg.get('text', '')
                                        else:
                                            text = str(msg)
                                        if text:
                                            session_info['first_messages'].append(text[:300])
                                
                                ws_sessions.append(session_info)
                        except:
                            pass
                
                if ws_sessions:
                    results['workspace_sessions'][workspace_name] = ws_sessions
            
            conn.close()
        except:
            pass
    
    return results

def print_sinhala_summary(results):
    """Print summary in Sinhala"""
    
    print("=" * 80)
    print("🔍 GITHUB COPILOT CHAT HISTORY - සාරාංශය (SUMMARY)")
    print("=" * 80)
    print(f"\n📅 Scan Date/දිනය: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print()
    
    print("📊 OVERALL STATISTICS / සමස්ත සංඛ්‍යාලේඛන")
    print("-" * 60)
    print(f"   🗨️  මුළු Chat Messages ගණන: {results['total_messages']}")
    print(f"   📁 Global Sessions ගණන: {len(results['global_sessions'])}")
    print(f"   🏢 Workspaces with Chat: {len(results['workspace_sessions'])}")
    print()
    
    print("📑 TOPICS / මාතෘකා (ඔබ කතා කළ දේවල්)")
    print("-" * 60)
    for topic, count in sorted(results['topics'].items(), key=lambda x: -x[1]):
        bar = "█" * min(count // 2, 30)
        print(f"   {topic:20s}: {count:4d} {bar}")
    print()
    
    print("💬 GLOBAL CHAT SESSIONS / ප්‍රධාන Chat Sessions")
    print("-" * 60)
    
    for session in results['global_sessions'][:15]:
        title = session['title'][:50] if session['title'] else 'Untitled'
        msg_count = session['message_count']
        print(f"\n   📝 {title}")
        print(f"      Messages: {msg_count}")
        
        if session['messages']:
            first_msg = session['messages'][0][:100].replace('\n', ' ')
            print(f"      First: {first_msg}...")
    
    print("\n")
    print("🏢 WORKSPACE-SPECIFIC SESSIONS / Workspace Sessions")
    print("-" * 60)
    
    for ws_name, sessions in results['workspace_sessions'].items():
        total_msgs = sum(s['message_count'] for s in sessions)
        ws_display = ws_name.split('/')[-1] if '/' in ws_name else ws_name
        print(f"\n   📁 {ws_display}")
        print(f"      Sessions: {len(sessions)}, Total Messages: {total_msgs}")
        
        for session in sessions[:3]:
            title = session['title'][:40] if session['title'] else 'Untitled'
            print(f"      - {title} ({session['message_count']} msgs)")

    print("\n")
    print("=" * 80)
    print("📍 SINHALA SUMMARY / සිංහල සාරාංශය")
    print("=" * 80)
    print("""
[TARGET] ඔබේ C Partition එකේ GitHub Copilot Chat History:

[1] මුළු Chat Messages: """ + str(results['total_messages']) + """
   - ඔබ Copilot සමඟ ගොඩක් chat කරලා තියෙනවා!

[2] ප්‍රධාන වැඩ කරපු topics:""")
    
    for topic, count in sorted(results['topics'].items(), key=lambda x: -x[1])[:5]:
        print(f"   * {topic}: {count} messages")
    
    print("""
[3] Chat sessions save velaa thiyenne:
   * Global Storage: C:\\Users\\Cashi\\AppData\\Roaming\\Code\\User\\globalStorage\\state.vscdb
   * Workspace Storage folders: C:\\Users\\Cashi\\AppData\\Roaming\\Code\\User\\workspaceStorage\\

[4] Obe main projects:
   * whatsapp-sql-api - WhatsApp bot & Invoice system
   * lasantha-tire - Dashboard project
   * Crystal Reports - Report generation
   * Peachtree - Accounting integration

[5] Oba Copilot ekka vedi vashyen:
   * Bug fixes / Errors fix karanna
   * API development
   * Database/SQL queries
   * WhatsApp integration
   * Invoice/Billing systems

[NOTE] Me data VS Code restart kalat save vela thiyenawa.
   Obata parana conversations back access karanna puluwan!
""")

if __name__ == '__main__':
    results = scan_all_chat_history()
    print_sinhala_summary(results)

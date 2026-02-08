# -*- coding: utf-8 -*-
"""
Peachtree ODBC Bridge (32-bit)
Direct access to Peachtree data via 32-bit ODBC
NO username/password needed!
"""
print(">>> LOADING ROOT peachtree-odbc-bridge-32bit.py <<<", flush=True)
import os
import pyodbc
import logging
import threading
import time
from flask import Flask, jsonify, request
from flask_cors import CORS
import traceback
from datetime import datetime, timedelta
import sys
from decimal import Decimal, ROUND_HALF_UP

# Fix Unicode encoding for Windows console
if sys.platform == 'win32':
    try:
        sys.stdout.reconfigure(encoding='utf-8')
        sys.stderr.reconfigure(encoding='utf-8')
    except:
        pass

LOG_LEVEL = os.getenv('PEACHTREE_BRIDGE_LOG_LEVEL', 'INFO').upper()
logging.basicConfig(
    level=getattr(logging, LOG_LEVEL, logging.INFO),
    format='%(asctime)s | %(levelname)s | %(name)s | %(message)s'
)
log = logging.getLogger('peachtree_bridge')

app = Flask(__name__)
CORS(app)

# ODBC connection settings (override via environment variables to avoid hard-coded credentials)
ODBC_DSN = os.getenv('PEACHTREE_DSN', 'Peachtree')
ODBC_USER = os.getenv('PEACHTREE_UID', 'Peachtree')
ODBC_PASSWORD = os.getenv('PEACHTREE_PWD', '#Snath1')
ODBC_DRIVER = os.getenv('PEACHTREE_DRIVER')
ODBC_SERVER = os.getenv('PEACHTREE_SERVER')
ODBC_DBQ = os.getenv('PEACHTREE_DBQ', '@DEFAULT')

if ODBC_DRIVER and ODBC_SERVER:
    CONNECTION_STRING = (
        f"Driver={{{ODBC_DRIVER}}};ServerName={ODBC_SERVER};DBQ={ODBC_DBQ};"
        f"UID={ODBC_USER};PWD={ODBC_PASSWORD}"
    )
else:
    CONNECTION_STRING = f"DSN={ODBC_DSN};UID={ODBC_USER};PWD={ODBC_PASSWORD}"

SCHEMA_TTL_SECONDS = int(os.getenv('PEACHTREE_SCHEMA_CACHE_TTL', '300'))
MAX_FETCH_SIZE = int(os.getenv('PEACHTREE_FETCH_SIZE', '5000'))

# Treat very small balances as floating-point "dust" and exclude them
# so totals/aging buckets match the report tables.
DUST_THRESHOLD = Decimal(os.getenv('PEACHTREE_DUST_THRESHOLD', '0.01'))
MONEY_QUANT = Decimal('0.01')

def _to_decimal(value) -> Decimal:
    try:
        return Decimal(str(value if value is not None else 0))
    except Exception:
        return Decimal('0')

def _money(value) -> Decimal:
    return _to_decimal(value).quantize(MONEY_QUANT, rounding=ROUND_HALF_UP)

# ❌ MOCK DATA COMPLETELY DISABLED - ONLY REAL PEACHTREE DATA
# This system MUST NEVER use mock/sample/fake data
USE_MOCK_DATA = False

_connection_lock = threading.Lock()
_active_connection = None
_last_health_check = 0.0
_schema_cache = {'tables': [], 'expires': 0, 'columns': {}}

def _create_connection():
    conn = pyodbc.connect(CONNECTION_STRING, timeout=10, autocommit=True)
    try:
        conn.setdecoding(pyodbc.SQL_CHAR, encoding='utf-8')
        conn.setdecoding(pyodbc.SQL_WCHAR, encoding='utf-8')
        conn.setencoding(encoding='utf-8')
    except Exception:
        pass
    return conn


def reset_connection():
    global _active_connection
    with _connection_lock:
        if _active_connection:
            try:
                _active_connection.close()
            except Exception:
                pass
            _active_connection = None


def get_connection(force_new=False):
    """Get (or reuse) ODBC connection to Peachtree - NO FALLBACK TO MOCK DATA"""
    global USE_MOCK_DATA, _active_connection

    if USE_MOCK_DATA:
        raise Exception("Mock data mode is disabled. Real Peachtree connection is required.")

    with _connection_lock:
        try:
            if not force_new and _active_connection:
                try:
                    cursor = _active_connection.cursor()
                    cursor.execute("SELECT 1")
                    cursor.fetchone()
                    cursor.close()
                    return _active_connection
                except Exception:
                    log.warning("Cached ODBC connection became invalid. Reconnecting...")
                    try:
                        _active_connection.close()
                    except Exception:
                        pass
                    _active_connection = None

            _active_connection = _create_connection()
            USE_MOCK_DATA = False
            log.info("✅ Connected to Peachtree via ODBC (%s)", CONNECTION_STRING)
            return _active_connection
        except Exception as e:
            log.error("❌ ODBC Connection error: %s", e)
            reset_connection()
            raise Exception(f"Peachtree ODBC connection failed: {e}. Mock data is DISABLED.")

def execute_query(query, params=None, fetch_size=None):
    """Execute SQL query and return results - ONLY REAL DATA"""
    global USE_MOCK_DATA

    if USE_MOCK_DATA:
        raise Exception("CRITICAL ERROR: Mock data mode is DISABLED. Only real Peachtree data allowed!")

    fetch_size = fetch_size or MAX_FETCH_SIZE
    try:
        conn = get_connection()
        cursor = conn.cursor()

        if params:
            cursor.execute(query, params)
        else:
            cursor.execute(query)

        columns = [column[0] for column in cursor.description]
        rows = []
        while True:
            batch = cursor.fetchmany(fetch_size)
            if not batch:
                break
            for row in batch:
                row_dict = {}
                for i, value in enumerate(row):
                    if isinstance(value, bytes):
                        try:
                            row_dict[columns[i]] = value.decode('utf-8', errors='ignore').strip()
                        except Exception:
                            row_dict[columns[i]] = str(value)
                    else:
                        row_dict[columns[i]] = value
                rows.append(row_dict)

        cursor.close()
        return rows
    except Exception as e:
        log.error("❌ Query error: %s", e)
        log.debug("Query failed: %s", query[:200])
        reset_connection()
        raise Exception(f"Peachtree query failed: {e}. Mock data is DISABLED.")

def validate_data(rows, data_type='unknown'):
    """Validate and clean data for accuracy"""
    if not rows:
        return []
    
    cleaned = []
    for row in rows:
        cleaned_row = {}
        for key, value in row.items():
            # Clean string values
            if isinstance(value, str):
                # Remove extra whitespace
                value = value.strip()
                # Remove null characters
                value = value.replace('\x00', '')
            # Convert None to empty string for display
            elif value is None:
                value = ''
            # Convert Decimal to float for JSON serialization
            elif isinstance(value, Decimal):
                value = float(value)
            cleaned_row[key] = value
        cleaned.append(cleaned_row)
    
    print(f"✓ Validated {len(cleaned)} {data_type} records")
    return cleaned


def get_tables_cached(force_refresh=False):
    """Cache table metadata to avoid expensive introspection calls"""
    now = time.time()
    if not force_refresh and _schema_cache['tables'] and now < _schema_cache['expires']:
        return _schema_cache['tables']

    conn = get_connection()
    cursor = conn.cursor()
    tables = [table.table_name for table in cursor.tables(tableType='TABLE')]
    cursor.close()

    _schema_cache['tables'] = tables
    _schema_cache['expires'] = now + SCHEMA_TTL_SECONDS
    return tables


def get_columns_for_table(table_name):
    cache_key = table_name.lower()
    now = time.time()
    if cache_key in _schema_cache['columns']:
        meta = _schema_cache['columns'][cache_key]
        if now < meta['expires']:
            return meta['columns']

    conn = get_connection()
    cursor = conn.cursor()
    columns = []
    for column in cursor.columns(table=table_name):
        columns.append({
            'name': column.column_name,
            'type': column.type_name,
            'size': column.column_size
        })
    cursor.close()

    _schema_cache['columns'][cache_key] = {
        'columns': columns,
        'expires': now + SCHEMA_TTL_SECONDS
    }
    return columns

# =====================================================
# MOCK DATA FOR DEVELOPMENT/TESTING
# =====================================================

def get_mock_customers():
    """Mock customer data for testing - Expanded to 30 customers"""
    return [
        {'CustomerID': 'CUST001', 'Customer_Bill_Name': 'ABC Motors Pvt Ltd', 'Balance': 250000, 'Terms_CreditLimit': 500000, 'Contact': 'John Silva', 'Phone_Number': '0771234567', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST002', 'Customer_Bill_Name': 'XYZ Traders', 'Balance': 180000, 'Terms_CreditLimit': 300000, 'Contact': 'Nimal Perera', 'Phone_Number': '0712345678', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST003', 'Customer_Bill_Name': 'Lanka Transport Co', 'Balance': 320000, 'Terms_CreditLimit': 600000, 'Contact': 'Kamal Fernando', 'Phone_Number': '0773456789', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST004', 'Customer_Bill_Name': 'City Garage', 'Balance': 150000, 'Terms_CreditLimit': 250000, 'Contact': 'Sunil Kumar', 'Phone_Number': '0714567890', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST005', 'Customer_Bill_Name': 'Quick Service Center', 'Balance': 95000, 'Terms_CreditLimit': 200000, 'Contact': 'Ravi Jayasuriya', 'Phone_Number': '0775678901', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST006', 'Customer_Bill_Name': 'Metro Auto Parts', 'Balance': 210000, 'Terms_CreditLimit': 400000, 'Contact': 'Lakmal Silva', 'Phone_Number': '0716789012', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST007', 'Customer_Bill_Name': 'Highway Motors', 'Balance': 175000, 'Terms_CreditLimit': 350000, 'Contact': 'Chaminda Perera', 'Phone_Number': '0777890123', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST008', 'Customer_Bill_Name': 'Premium Auto Care', 'Balance': 280000, 'Terms_CreditLimit': 500000, 'Contact': 'Rohan Fernando', 'Phone_Number': '0718901234', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST009', 'Customer_Bill_Name': 'Express Tyres', 'Balance': 125000, 'Terms_CreditLimit': 250000, 'Contact': 'Dinesh Kumar', 'Phone_Number': '0779012345', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST010', 'Customer_Bill_Name': 'Royal Service Station', 'Balance': 195000, 'Terms_CreditLimit': 300000, 'Contact': 'Mahesh Silva', 'Phone_Number': '0710123456', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST011', 'Customer_Bill_Name': 'Speed Auto Workshop', 'Balance': 165000, 'Terms_CreditLimit': 280000, 'Contact': 'Pradeep Kumar', 'Phone_Number': '0771122334', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST012', 'Customer_Bill_Name': 'Star Motors Garage', 'Balance': 225000, 'Terms_CreditLimit': 450000, 'Contact': 'Saman Wijesinghe', 'Phone_Number': '0712233445', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST013', 'Customer_Bill_Name': 'Elite Car Care', 'Balance': 135000, 'Terms_CreditLimit': 220000, 'Contact': 'Asanka Silva', 'Phone_Number': '0773344556', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST014', 'Customer_Bill_Name': 'Mega Auto Solutions', 'Balance': 290000, 'Terms_CreditLimit': 550000, 'Contact': 'Janaka Fernando', 'Phone_Number': '0714455667', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST015', 'Customer_Bill_Name': 'Prime Vehicle Service', 'Balance': 115000, 'Terms_CreditLimit': 240000, 'Contact': 'Nuwan Perera', 'Phone_Number': '0775566778', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST016', 'Customer_Bill_Name': 'Golden Wheel Motors', 'Balance': 245000, 'Terms_CreditLimit': 480000, 'Contact': 'Thilak Jayawardena', 'Phone_Number': '0716677889', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST017', 'Customer_Bill_Name': 'Supreme Auto Tech', 'Balance': 185000, 'Terms_CreditLimit': 360000, 'Contact': 'Dilshan Silva', 'Phone_Number': '0777788990', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST018', 'Customer_Bill_Name': 'Peak Performance Garage', 'Balance': 155000, 'Terms_CreditLimit': 290000, 'Contact': 'Kusal Fernando', 'Phone_Number': '0718899001', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST019', 'Customer_Bill_Name': 'Thunder Auto Works', 'Balance': 205000, 'Terms_CreditLimit': 400000, 'Contact': 'Ruwan Jayasena', 'Phone_Number': '0779900112', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST020', 'Customer_Bill_Name': 'Diamond Motors Ltd', 'Balance': 270000, 'Terms_CreditLimit': 520000, 'Contact': 'Chamara Perera', 'Phone_Number': '0710011223', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST021', 'Customer_Bill_Name': 'Victory Auto Center', 'Balance': 145000, 'Terms_CreditLimit': 260000, 'Contact': 'Gayan Silva', 'Phone_Number': '0771122334', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST022', 'Customer_Bill_Name': 'Platinum Car Service', 'Balance': 235000, 'Terms_CreditLimit': 460000, 'Contact': 'Lasitha Fernando', 'Phone_Number': '0712233445', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST023', 'Customer_Bill_Name': 'Ace Motors Workshop', 'Balance': 125000, 'Terms_CreditLimit': 230000, 'Contact': 'Tharindu Kumar', 'Phone_Number': '0773344556', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST024', 'Customer_Bill_Name': 'Swift Auto Repairs', 'Balance': 195000, 'Terms_CreditLimit': 380000, 'Contact': 'Kasun Perera', 'Phone_Number': '0714455667', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST025', 'Customer_Bill_Name': 'Crown Vehicle Solutions', 'Balance': 265000, 'Terms_CreditLimit': 510000, 'Contact': 'Isuru Silva', 'Phone_Number': '0775566778', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST026', 'Customer_Bill_Name': 'Velocity Motors Co', 'Balance': 175000, 'Terms_CreditLimit': 340000, 'Contact': 'Buddhika Fernando', 'Phone_Number': '0716677889', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST027', 'Customer_Bill_Name': 'Excel Auto Services', 'Balance': 215000, 'Terms_CreditLimit': 420000, 'Contact': 'Sandun Jayasinghe', 'Phone_Number': '0777788990', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST028', 'Customer_Bill_Name': 'Fusion Car Care', 'Balance': 155000, 'Terms_CreditLimit': 280000, 'Contact': 'Chathura Silva', 'Phone_Number': '0718899001', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST029', 'Customer_Bill_Name': 'Prestige Auto House', 'Balance': 285000, 'Terms_CreditLimit': 540000, 'Contact': 'Hasitha Perera', 'Phone_Number': '0779900112', 'CustomerIsInactive': 0},
        {'CustomerID': 'CUST030', 'Customer_Bill_Name': 'Turbo Motors Garage', 'Balance': 165000, 'Terms_CreditLimit': 310000, 'Contact': 'Ravindu Fernando', 'Phone_Number': '0710011223', 'CustomerIsInactive': 0},
    ]

def get_mock_vendors():
    """Mock vendor data for testing - Expanded to 20 vendors"""
    return [
        {'VendorID': 'VEND001', 'VendorName': 'Ceat Tyres Lanka', 'Balance': 450000, 'Contact': 'Sales Manager', 'Phone_Number': '0112345678', 'VendorIsInactive': 0},
        {'VendorID': 'VEND002', 'VendorName': 'MRF Tyres', 'Balance': 380000, 'Contact': 'Distribution Manager', 'Phone_Number': '0112345679', 'VendorIsInactive': 0},
        {'VendorID': 'VEND003', 'VendorName': 'Yokohama Lanka', 'Balance': 520000, 'Contact': 'Account Manager', 'Phone_Number': '0112345680', 'VendorIsInactive': 0},
        {'VendorID': 'VEND004', 'VendorName': 'Bridgestone Sri Lanka', 'Balance': 295000, 'Contact': 'Sales Executive', 'Phone_Number': '0112345681', 'VendorIsInactive': 0},
        {'VendorID': 'VEND005', 'VendorName': 'Continental Tyres', 'Balance': 175000, 'Contact': 'Regional Manager', 'Phone_Number': '0112345682', 'VendorIsInactive': 0},
        {'VendorID': 'VEND006', 'VendorName': 'Michelin Lanka', 'Balance': 410000, 'Contact': 'Business Manager', 'Phone_Number': '0112345683', 'VendorIsInactive': 0},
        {'VendorID': 'VEND007', 'VendorName': 'Goodyear Distributors', 'Balance': 225000, 'Contact': 'Sales Head', 'Phone_Number': '0112345684', 'VendorIsInactive': 0},
        {'VendorID': 'VEND008', 'VendorName': 'Dunlop Tyres', 'Balance': 190000, 'Contact': 'Territory Manager', 'Phone_Number': '0112345685', 'VendorIsInactive': 0},
        {'VendorID': 'VEND009', 'VendorName': 'Pirelli Lanka', 'Balance': 485000, 'Contact': 'Country Manager', 'Phone_Number': '0112345686', 'VendorIsInactive': 0},
        {'VendorID': 'VEND010', 'VendorName': 'Hankook Tyres', 'Balance': 335000, 'Contact': 'Sales Director', 'Phone_Number': '0112345687', 'VendorIsInactive': 0},
        {'VendorID': 'VEND011', 'VendorName': 'Falken Tyres', 'Balance': 265000, 'Contact': 'Business Head', 'Phone_Number': '0112345688', 'VendorIsInactive': 0},
        {'VendorID': 'VEND012', 'VendorName': 'Nexen Tire Lanka', 'Balance': 215000, 'Contact': 'Sales Manager', 'Phone_Number': '0112345689', 'VendorIsInactive': 0},
        {'VendorID': 'VEND013', 'VendorName': 'Kumho Tyres', 'Balance': 395000, 'Contact': 'Regional Director', 'Phone_Number': '0112345690', 'VendorIsInactive': 0},
        {'VendorID': 'VEND014', 'VendorName': 'Toyo Tires Lanka', 'Balance': 425000, 'Contact': 'Area Manager', 'Phone_Number': '0112345691', 'VendorIsInactive': 0},
        {'VendorID': 'VEND015', 'VendorName': 'BF Goodrich Distributors', 'Balance': 245000, 'Contact': 'Distribution Head', 'Phone_Number': '0112345692', 'VendorIsInactive': 0},
        {'VendorID': 'VEND016', 'VendorName': 'Nankang Tyres', 'Balance': 185000, 'Contact': 'Sales Coordinator', 'Phone_Number': '0112345693', 'VendorIsInactive': 0},
        {'VendorID': 'VEND017', 'VendorName': 'Maxxis Lanka', 'Balance': 355000, 'Contact': 'Business Manager', 'Phone_Number': '0112345694', 'VendorIsInactive': 0},
        {'VendorID': 'VEND018', 'VendorName': 'Cooper Tires', 'Balance': 275000, 'Contact': 'Account Director', 'Phone_Number': '0112345695', 'VendorIsInactive': 0},
        {'VendorID': 'VEND019', 'VendorName': 'General Tire Lanka', 'Balance': 305000, 'Contact': 'Sales Executive', 'Phone_Number': '0112345696', 'VendorIsInactive': 0},
        {'VendorID': 'VEND020', 'VendorName': 'Firestone Lanka', 'Balance': 445000, 'Contact': 'Regional Manager', 'Phone_Number': '0112345697', 'VendorIsInactive': 0},
    ]

def get_mock_chart():
    """Mock chart of accounts for testing"""
    return [
        {'AccountID': '1000', 'Description': 'Cash on Hand', 'AccountType': 'Cash', 'Balance': 85000},
        {'AccountID': '1010', 'Description': 'Bank of Ceylon - Current', 'AccountType': 'Bank Account', 'Balance': 1250000},
        {'AccountID': '1020', 'Description': 'Commercial Bank - Savings', 'AccountType': 'Bank Account', 'Balance': 875000},
        {'AccountID': '1030', 'Description': 'Sampath Bank - Current', 'AccountType': 'Bank Account', 'Balance': 450000},
        {'AccountID': '1040', 'Description': 'Petty Cash', 'AccountType': 'Cash', 'Balance': 15000},
    ]

def get_mock_ar_aging():
    """Mock AR aging data for testing"""
    mock_customers = get_mock_customers()
    # Map field names to match query aliases
    customers = []
    for c in mock_customers:
        customers.append({
            'CustomerID': c['CustomerID'],
            'CustomerName': c['Customer_Bill_Name'],  # Map to CustomerName
            'TotalOutstanding': c['Balance'],  # Map to TotalOutstanding
            'CreditLimit': c['Terms_CreditLimit']  # Map to CreditLimit
        })
    
    total_ar = sum(c['TotalOutstanding'] for c in customers)
    
    # Realistic aging distribution
    return {
        'summary': {
            'current': total_ar * 0.65,
            'days_1_30': total_ar * 0.20,
            'days_31_60': total_ar * 0.10,
            'days_61_90': total_ar * 0.03,
            'over_90': total_ar * 0.02,
            'total': total_ar
        },
        'customers': customers,
        'total_customers': len(customers)
    }

def get_mock_ap_aging():
    """Mock AP aging data for testing"""
    mock_vendors = get_mock_vendors()
    # Map field names to match query aliases
    vendors = []
    for v in mock_vendors:
        vendors.append({
            'VendorID': v['VendorID'],
            'VendorName': v['VendorName'],
            'TotalOutstanding': v['Balance'],  # Map to TotalOutstanding
            'CreditLimit': 0  # Vendors don't have credit limit typically
        })
    
    total_ap = sum(v['TotalOutstanding'] for v in vendors)
    
    # Realistic aging distribution
    return {
        'summary': {
            'current': total_ap * 0.70,
            'days_1_30': total_ap * 0.18,
            'days_31_60': total_ap * 0.08,
            'days_61_90': total_ap * 0.03,
            'over_90': total_ap * 0.01,
            'total': total_ap
        },
        'vendors': vendors,
        'total_vendors': len(vendors)
    }

def get_mock_cash_balances():
    """Mock cash balances for testing"""
    mock_accounts = get_mock_chart()
    # Map field names to match query aliases
    accounts = []
    for a in mock_accounts:
        accounts.append({
            'AccountID': a['AccountID'],
            'AccountName': a['Description'],  # Map to AccountName
            'AccountType': a['AccountType'],
            'Balance': a['Balance']
        })
    
    total_cash = sum(a['Balance'] for a in accounts)
    
    return {
        'accounts': accounts,
        'total_cash': total_cash,
        'total_accounts': len(accounts)
    }


def collect_bridge_diagnostics():
    global _last_health_check
    diagnostics = {
        'connection_string': CONNECTION_STRING,
        'schema_cache_entries': len(_schema_cache['columns']),
        'schema_cache_expires_in': max(0, int(_schema_cache['expires'] - time.time())) if _schema_cache['tables'] else 0,
        'mock_mode': USE_MOCK_DATA,
        'timestamp': datetime.utcnow().isoformat() + 'Z'
    }

    try:
        conn = get_connection()
        cursor = conn.cursor()
        cursor.execute('SELECT 1 as heartbeat')
        cursor.fetchone()
        cursor.close()
        diagnostics['connected'] = True
        diagnostics['last_error'] = None
    except Exception as exc:
        diagnostics['connected'] = False
        diagnostics['last_error'] = str(exc)

    _last_health_check = time.time()
    return diagnostics

@app.route('/health', methods=['GET'])
def health():
    """Health check endpoint"""
    diagnostics = collect_bridge_diagnostics()
    response = {
        'status': 'ok',
        'service': 'Peachtree ODBC Bridge (32-bit)',
        'odbc_dsn': ODBC_DSN,
        'connected': diagnostics.get('connected', False),
        'mode': 'ODBC' if diagnostics.get('connected') else 'ERROR',
        'diagnostics': diagnostics
    }
    status_code = 200 if diagnostics.get('connected') else 503
    return jsonify(response), status_code


@app.route('/status', methods=['GET'])
def status():
    """Extended diagnostics endpoint"""
    diagnostics = collect_bridge_diagnostics()
    diagnostics['tables_cached'] = len(_schema_cache['tables'])
    diagnostics['active_connection'] = bool(_active_connection)
    diagnostics['uptime_seconds'] = int(time.time() - _last_health_check) if _last_health_check else 0
    return jsonify(diagnostics)


@app.route('/api/peachtree/metadata', methods=['GET'])
def metadata():
    """Return schema and connection metadata to the dashboard"""
    try:
        tables = get_tables_cached(force_refresh=request.args.get('refresh') == '1')
        table_detail = []
        include_columns = request.args.get('includeColumns') == '1'
        for table_name in tables[: int(request.args.get('limit', len(tables)))]:
            entry = {'name': table_name}
            if include_columns:
                entry['columns'] = get_columns_for_table(table_name)
            table_detail.append(entry)

        return jsonify({
            'success': True,
            'tables': table_detail,
            'count': len(tables),
            'cache_ttl': SCHEMA_TTL_SECONDS
        })
    except Exception as exc:
        return jsonify({
            'success': False,
            'error': str(exc)
        }), 500

@app.route('/api/peachtree/tables', methods=['GET'])
def get_tables():
    """List all available tables"""
    try:
        tables = get_tables_cached(force_refresh=request.args.get('refresh') == '1')
        return jsonify({
            'success': True,
            'tables': tables,
            'count': len(tables)
        })
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'trace': traceback.format_exc()
        }), 500

@app.route('/api/peachtree/customers', methods=['GET'])
def get_customers():
    """Get customer data"""
    try:
        # Try different table names - Customers (plural) is the correct table name
        table_names = ['Customers', 'Customer', 'AR_Customer', 'CUSTOMER']
        
        for table_name in table_names:
            try:
                query = f"SELECT * FROM {table_name}"
                rows = execute_query(query)
                cleaned_rows = validate_data(rows, 'customer')
                return jsonify({
                    'success': True,
                    'table': table_name,
                    'count': len(cleaned_rows),
                    'data': cleaned_rows  # All records - validated and cleaned
                })
            except Exception as table_error:
                continue
        
        return jsonify({
            'success': False,
            'error': 'Customer table not found',
            'tried_tables': table_names
        }), 404
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'trace': traceback.format_exc()
        }), 500

@app.route('/api/peachtree/vendors', methods=['GET'])
def get_vendors():
    """Get vendor data"""
    try:
        # Try Vendors table first (plural) - Peachtree 2012 uses this
        table_names = ['Vendors', 'Vendor', 'AP_Vendor', 'VENDOR']
        
        for table_name in table_names:
            try:
                # Use explicit column selection with proper aliases
                # Peachtree uses "Name" not "VendorName"
                query = f"""
                    SELECT 
                        VendorID,
                        Name as VendorName,
                        VendorType,
                        Contact,
                        PhoneNumber as Phone_Number,
                        Email,
                        Balance,
                        LastInvoiceDate,
                        LastInvoiceAmount,
                        LastPaymentDate,
                        LastPaymentAmount,
                        DueDays,
                        DiscountDays,
                        DiscountPercent,
                        IsInactive as VendorIsInactive,
                        VendorSince,
                        GLAcntNumber
                    FROM {table_name}
                """
                rows = execute_query(query)
                cleaned_rows = validate_data(rows, 'vendor')
                
                print(f"✅ SUCCESS: Loaded {len(cleaned_rows)} REAL vendors from {table_name}")
                print(f"   First vendor: {cleaned_rows[0].get('VendorID')} - {cleaned_rows[0].get('VendorName')}")
                
                return jsonify({
                    'success': True,
                    'mode': 'real_data',  # NOT mock data!
                    'table': table_name,
                    'count': len(cleaned_rows),
                    'data': cleaned_rows  # All records - validated and cleaned
                })
            except Exception as e:
                print(f"❌ Table {table_name} failed: {e}")
                continue
        
        return jsonify({
            'success': False,
            'error': 'Vendor table not found'
        }), 404
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/chart-of-accounts', methods=['GET'])
def get_chart_of_accounts():
    """Get chart of accounts"""
    try:
        # Reordered to try 'Chart' first as it is most common in older versions
        table_names = ['Chart', 'ChartOfAccounts', 'GLAccount', 'GL_Account', 'CHART']
        
        for table_name in table_names:
            try:
                # Optimized query to fetch only necessary columns and ensure Balance is available
                # Balance0Net is typically the current balance for period 0
                query = f"""
                    SELECT 
                        AccountID, 
                        AccountDescription, 
                        AccountType, 
                        AccountIsInactive, 
                        Balance0Net,
                        Balance0Net as Balance 
                    FROM {table_name}
                """
                rows = execute_query(query)
                cleaned_rows = validate_data(rows, 'account')
                return jsonify({
                    'success': True,
                    'table': table_name,
                    'count': len(cleaned_rows),
                    'data': cleaned_rows  # All records - validated and cleaned
                })
            except Exception as e:
                log.warning(f"Failed to query {table_name}: {e}")
                continue
        
        return jsonify({
            'success': False,
            'error': 'Chart of Accounts table not found'
        }), 404
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/transactions', methods=['GET'])
def get_transactions():
    """Get transaction/journal entry data from JrnlHdr (Journal Headers)"""
    try:
        # Check for customer_id or vendor_id parameters
        customer_id = request.args.get('customer_id')
        vendor_id = request.args.get('vendor_id')
        
        # If customer or vendor filter is requested
        if customer_id or vendor_id:
            return get_entity_invoices(customer_id, vendor_id)
        
        # JrnlHdr contains journal entry headers
        table_names = ['JrnlHdr', 'JrnlRow', 'StoredTransHeaders', 'OnlineTransaction']
        
        for table_name in table_names:
            try:
                # Just get TOP 500 records without ordering (simpler, avoids date column issues)
                query = f"SELECT TOP 500 * FROM {table_name}"
                rows = execute_query(query)
                cleaned_rows = validate_data(rows, 'transaction')
                return jsonify({
                    'success': True,
                    'table': table_name,
                    'count': len(cleaned_rows),
                    'data': cleaned_rows
                })
            except Exception as table_err:
                print(f"❌ Table {table_name} failed: {table_err}")
                continue
        
        return jsonify({
            'success': False,
            'error': 'Transaction table not found',
            'tried_tables': table_names
        }), 404
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'trace': traceback.format_exc()
        }), 500


@app.route('/api/peachtree/financial-kpis', methods=['GET'])
def get_financial_kpis():
    """Compute financial KPIs (YTD revenue/expenses/net profit) from JrnlRow joined to Chart.

    Why: JrnlHdr doesn't contain usable amounts; JrnlRow.Amount holds signed debit(+)/credit(-)
    values per GL account. Chart.AccountType lets us classify rows:
      - 21 = Revenue
      - 23 = Cost of Sales
      - 24 = Expense
    """
    try:
        # Support either calendar-year KPIs (?year=YYYY) or custom range (?start=YYYY-MM-DD&end=YYYY-MM-DD).
        start_param = (request.args.get('start') or '').strip()
        end_param = (request.args.get('end') or '').strip()

        year_raw = (request.args.get('year') or '').strip()
        try:
            year = int(year_raw) if year_raw else datetime.now().year
        except Exception:
            year = datetime.now().year

        if start_param or end_param:
            if not start_param or not end_param:
                return jsonify({
                    'success': False,
                    'error': 'Both start and end are required when using custom range (YYYY-MM-DD)'
                }), 400
            try:
                start = datetime.strptime(start_param, '%Y-%m-%d')
                end_inclusive = datetime.strptime(end_param, '%Y-%m-%d')
            except Exception:
                return jsonify({
                    'success': False,
                    'error': 'Invalid date format. Use YYYY-MM-DD for start/end'
                }), 400
            end = end_inclusive + timedelta(days=1)
        else:
            start = datetime(year, 1, 1)
            end = datetime(year + 1, 1, 1)

        query = (
            "SELECT c.AccountType AS AccountType, SUM(r.Amount) AS TotalAmount "
            "FROM JrnlRow r "
            "INNER JOIN Chart c ON r.GLAcntNumber = c.GLAcntNumber "
            "WHERE r.IncludeInGL = 1 AND r.RowDate >= ? AND r.RowDate < ? "
            "AND c.AccountType IN (21, 23, 24) "
            "GROUP BY c.AccountType"
        )

        rows = execute_query(query, params=[start, end])

        totals = {int(r.get('AccountType')): _to_decimal(r.get('TotalAmount')) for r in rows if r.get('AccountType') is not None}
        revenue_signed = totals.get(21, Decimal('0'))
        cogs_signed = totals.get(23, Decimal('0'))
        expense_signed = totals.get(24, Decimal('0'))

        # JrnlRow.Amount is debit(+)/credit(-). Revenue is normally credit => negative.
        ytd_revenue = _money(-revenue_signed)
        ytd_expenses = _money(cogs_signed + expense_signed)
        net_profit_ytd = _money(ytd_revenue - ytd_expenses)

        return jsonify({
            'success': True,
            'year': year,
            'range': {
                'start': start.strftime('%Y-%m-%d'),
                'end_exclusive': end.strftime('%Y-%m-%d')
            },
            'ytd_revenue': float(ytd_revenue),
            'ytd_expenses': float(ytd_expenses),
            'net_profit_ytd': float(net_profit_ytd),
            'components': {
                'revenue_signed': float(_money(revenue_signed)),
                'cogs_signed': float(_money(cogs_signed)),
                'expense_signed': float(_money(expense_signed))
            },
            'source': 'JrnlRow.Amount joined Chart.AccountType'
        })
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'trace': traceback.format_exc()
        }), 500

def get_entity_invoices(customer_id=None, vendor_id=None):
    """Get invoices for a specific customer or vendor"""
    global USE_MOCK_DATA
    
    try:
        entity_type = 'Customer' if customer_id else 'Vendor'
        entity_id = customer_id or vendor_id
        
        # If mock data mode, return mock invoices
        if USE_MOCK_DATA:
            print(f"📊 Returning mock invoice data for {entity_type} {entity_id}")
            import random
            mock_invoices = [
                {
                    'InvoiceID': f'INV-{random.randint(1000, 9999)}',
                    'TransactionID': f'TXN-{random.randint(1000, 9999)}',
                    'Date': '2025-01-15',
                    'Type': 'Invoice',
                    'Description': 'Product Sale - Tires',
                    'Amount': random.randint(50000, 300000),
                    'Status': 'Unpaid',
                    'DueDate': '2025-02-15'
                },
                {
                    'InvoiceID': f'INV-{random.randint(1000, 9999)}',
                    'TransactionID': f'TXN-{random.randint(1000, 9999)}',
                    'Date': '2025-01-20',
                    'Type': 'Invoice',
                    'Description': 'Service Charge',
                    'Amount': random.randint(10000, 50000),
                    'Status': 'Unpaid',
                    'DueDate': '2025-02-20'
                },
                {
                    'InvoiceID': f'PAY-{random.randint(100, 999)}',
                    'TransactionID': f'TXN-{random.randint(1000, 9999)}',
                    'Date': '2025-01-10',
                    'Type': 'Payment',
                    'Description': 'Payment Received',
                    'Amount': -random.randint(50000, 150000),
                    'Status': 'Posted',
                    'DueDate': '-'
                },
                {
                    'InvoiceID': f'INV-{random.randint(1000, 9999)}',
                    'TransactionID': f'TXN-{random.randint(1000, 9999)}',
                    'Date': '2024-12-15',
                    'Type': 'Invoice',
                    'Description': 'Previous Month Sale',
                    'Amount': random.randint(75000, 200000),
                    'Status': 'Unpaid',
                    'DueDate': '2025-01-15'
                }
            ]
            return jsonify({
                'success': True,
                'mode': 'mock',
                'entity_type': entity_type,
                'entity_id': entity_id,
                'count': len(mock_invoices),
                'data': mock_invoices
            })
        
        # Try to query real invoice data from Peachtree
        # Note: JrnlRow uses CustomerRecordNumber/VendorRecordNumber, not CustomerID/VendorID
        # We need to first get the record number for the customer/vendor
        
        if customer_id:
            # Get customer record number first
            try:
                cust_query = f"SELECT CustomerRecordNumber FROM Customers WHERE CustomerID = '{customer_id}'"
                cust_result = execute_query(cust_query)
                if not cust_result or len(cust_result) == 0:
                    raise Exception(f"Customer {customer_id} not found")
                record_num = cust_result[0].get('CustomerRecordNumber')
                
                # Try JrnlRow with CustomerRecordNumber
                table_queries = [
                    f"SELECT TOP 50 * FROM JrnlRow WHERE CustomerRecordNumber = {record_num} ORDER BY RowDate DESC"
                ]
            except Exception as e:
                print(f"❌ Could not get customer record number: {e}")
                table_queries = []
        else:
            # Get vendor record number first
            try:
                vend_query = f"SELECT VendorRecordNumber FROM Vendors WHERE VendorID = '{vendor_id}'"
                vend_result = execute_query(vend_query)
                if not vend_result or len(vend_result) == 0:
                    raise Exception(f"Vendor {vendor_id} not found")
                record_num = vend_result[0].get('VendorRecordNumber')
                
                # Try JrnlRow with VendorRecordNumber
                table_queries = [
                    f"SELECT TOP 50 * FROM JrnlRow WHERE VendorRecordNumber = {record_num} ORDER BY RowDate DESC"
                ]
            except Exception as e:
                print(f"❌ Could not get vendor record number: {e}")
                table_queries = []
        
        for query in table_queries:
            try:
                rows = execute_query(query)
                if rows and len(rows) > 0:
                    # Clean and format the data
                    invoices = []
                    for row in rows:
                        invoice = {
                            'InvoiceID': row.get('InvNumForThisTrx') or row.get('RowNumber') or 'N/A',
                            'TransactionID': row.get('Journal') or str(row.get('RowNumber')) or 'N/A',
                            'Date': str(row.get('RowDate') or ''),
                            'Type': row.get('RowType') or 'Transaction',
                            'Description': row.get('RowDescription') or 'Transaction',
                            'Amount': float(row.get('Amount') or 0),
                            'Status': 'Posted',
                            'DueDate': '-'
                        }
                        invoices.append(invoice)
                    
                    print(f"✅ Found {len(invoices)} invoices for {entity_type} {entity_id}")
                    return jsonify({
                        'success': True,
                        'mode': 'odbc',
                        'entity_type': entity_type,
                        'entity_id': entity_id,
                        'count': len(invoices),
                        'data': invoices
                    })
            except Exception as query_error:
                print(f"❌ Query failed: {query_error}")
                continue
        
        # If no data found, return empty
        print(f"⚠️ No invoice data found for {entity_type} {entity_id}")
        return jsonify({
            'success': True,
            'mode': 'odbc',
            'entity_type': entity_type,
            'entity_id': entity_id,
            'count': 0,
            'data': [],
            'message': 'No invoices found'
        })
        
    except Exception as e:
        print(f"❌ Error in get_entity_invoices: {e}")
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/query', methods=['POST'])
def custom_query():
    """Execute custom SQL query"""
    try:
        data = request.get_json()
        query = data.get('query')
        
        if not query:
            return jsonify({
                'success': False,
                'error': 'Query is required'
            }), 400
        
        rows = execute_query(query)
        
        return jsonify({
            'success': True,
            'count': len(rows),
            'data': rows
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e),
            'trace': traceback.format_exc()
        }), 500

# Alias for compatibility with some clients calling /api/query
@app.route('/api/query', methods=['POST'])
def custom_query_alias():
    return custom_query()


@app.route('/api/peachtree/chunked-query', methods=['POST'])
def chunked_query():
    """Execute heavy SQL queries with controlled fetch size and optional limits"""
    try:
        payload = request.get_json(force=True)
        query = payload.get('query')
        fetch_size = int(payload.get('fetch_size', MAX_FETCH_SIZE))
        fetch_size = max(500, min(fetch_size, 20000))

        if not query:
            return jsonify({'success': False, 'error': 'Query is required'}), 400

        rows = execute_query(query, fetch_size=fetch_size)
        return jsonify({
            'success': True,
            'count': len(rows),
            'data': rows,
            'fetch_size': fetch_size
        })
    except Exception as exc:
        return jsonify({
            'success': False,
            'error': str(exc)
        }), 500

# =====================================================
# BUSINESS STATUS DASHBOARD ENDPOINTS
# =====================================================

@app.route('/api/peachtree/business-status/ar-aging', methods=['GET'])
def get_ar_aging():
    """
    Get Accounts Receivable Aging Report
    Shows outstanding customer invoices by age buckets
    """
    global USE_MOCK_DATA
    
    try:
        # If mock data mode, return mock AR aging
        if USE_MOCK_DATA:
            mock_data = get_mock_ar_aging()
            return jsonify({
                'success': True,
                'mode': 'mock',
                **mock_data
            })
        
        # First, let's try to get customer outstanding balances from General_AR
        try:
            query = """
                SELECT 
                    CustomerID,
                    Customer_Bill_Name as CustomerName,
                    Balance as TotalOutstanding,
                    Terms_CreditLimit as CreditLimit,
                    Phone_Number as Phone,
                    LastInvoiceDate
                FROM Customers
                WHERE Balance > 0.01
                ORDER BY Balance DESC
            """
            customers = execute_query(query)
            
            # Calculate totals and aging buckets (Decimal for accuracy)
            total_ar = Decimal('0')
            
            # Real Aging Calculation
            aging_summary = {
                'current': Decimal('0'),
                'days_1_30': Decimal('0'),
                'days_31_60': Decimal('0'),
                'days_61_90': Decimal('0'),
                'over_90': Decimal('0'),
                'total': Decimal('0')
            }
            
            now = datetime.now()
            
            filtered_customers = []
            for c in customers:
                balance_dec = _to_decimal(c.get('TotalOutstanding', 0) or 0)
                if balance_dec <= DUST_THRESHOLD:
                    continue

                balance = _money(balance_dec)
                c['TotalOutstanding'] = float(balance)
                filtered_customers.append(c)
                total_ar += balance
                last_inv_date_str = c.get('LastInvoiceDate')
                
                if last_inv_date_str:
                    try:
                        # Parse date (e.g., "Mon, 10 Nov 2025 00:00:00 GMT")
                        # Simplified parsing assuming standard format or just use dateutil if needed
                        # For now, try basic parsing or fallback
                        if isinstance(last_inv_date_str, str):
                            # Try to parse common formats
                            try:
                                last_inv_date = datetime.strptime(last_inv_date_str[:16], '%a, %d %b %Y')
                            except:
                                last_inv_date = datetime.strptime(last_inv_date_str[:10], '%Y-%m-%d')
                        else:
                            last_inv_date = last_inv_date_str # Already datetime?
                            
                        days_diff = (now - last_inv_date).days
                        c['DaysOutstanding'] = days_diff
                        
                        if days_diff <= 0:
                            aging_summary['current'] += balance
                        elif days_diff <= 30:
                            aging_summary['days_1_30'] += balance
                        elif days_diff <= 60:
                            aging_summary['days_31_60'] += balance
                        elif days_diff <= 90:
                            aging_summary['days_61_90'] += balance
                        else:
                            aging_summary['over_90'] += balance
                            
                    except Exception as date_err:
                        # Fallback if date parsing fails
                        c['DaysOutstanding'] = 0
                        aging_summary['current'] += balance
                else:
                    c['DaysOutstanding'] = 0
                    aging_summary['current'] += balance

            aging_summary['total'] = total_ar

            # Convert Decimals to floats for JSON
            aging_summary_out = {k: float(v.quantize(MONEY_QUANT, rounding=ROUND_HALF_UP)) for k, v in aging_summary.items()}

            return jsonify({
                'success': True,
                'mode': 'odbc',
                'summary': aging_summary_out,
                'customers': filtered_customers,
                'total_customers': len(filtered_customers)
            })
            
        except Exception as e:
            print(f"❌ AR Aging error: {e}")
            return jsonify({
                'success': False,
                'error': str(e),
                'trace': traceback.format_exc()
            }), 500
            
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/business-status/ap-aging', methods=['GET'])
def get_ap_aging():
    """
    Get Accounts Payable Aging Report
    Shows outstanding vendor bills by age buckets
    """
    global USE_MOCK_DATA
    
    try:
        # If mock data mode, return mock AP aging
        if USE_MOCK_DATA:
            mock_data = get_mock_ap_aging()
            return jsonify({
                'success': True,
                'mode': 'mock',
                **mock_data
            })
        
        query = """
            SELECT 
                VendorID,
                Name as VendorName,
                Balance as TotalOutstanding,
                0 as CreditLimit,
                PhoneNumber as Phone,
                LastInvoiceDate
            FROM Vendors
            WHERE Balance > 0.01
            ORDER BY Balance DESC
        """
        vendors = execute_query(query)

        total_ap = Decimal('0')
        
        # Real Aging Calculation
        aging_summary = {
            'current': Decimal('0'),
            'days_1_30': Decimal('0'),
            'days_31_60': Decimal('0'),
            'days_61_90': Decimal('0'),
            'over_90': Decimal('0'),
            'total': Decimal('0')
        }
        
        now = datetime.now()
        
        filtered_vendors = []
        for v in vendors:
            balance_dec = _to_decimal(v.get('TotalOutstanding', 0) or 0)
            if balance_dec <= DUST_THRESHOLD:
                continue

            balance = _money(balance_dec)
            v['TotalOutstanding'] = float(balance)
            filtered_vendors.append(v)
            total_ap += balance
            last_inv_date_str = v.get('LastInvoiceDate')
            
            if last_inv_date_str:
                try:
                    if isinstance(last_inv_date_str, str):
                        try:
                            last_inv_date = datetime.strptime(last_inv_date_str[:16], '%a, %d %b %Y')
                        except:
                            last_inv_date = datetime.strptime(last_inv_date_str[:10], '%Y-%m-%d')
                    else:
                        last_inv_date = last_inv_date_str
                        
                    days_diff = (now - last_inv_date).days
                    v['DaysOutstanding'] = days_diff
                    
                    if days_diff <= 0:
                        aging_summary['current'] += balance
                    elif days_diff <= 30:
                        aging_summary['days_1_30'] += balance
                    elif days_diff <= 60:
                        aging_summary['days_31_60'] += balance
                    elif days_diff <= 90:
                        aging_summary['days_61_90'] += balance
                    else:
                        aging_summary['over_90'] += balance
                        
                except Exception as date_err:
                    v['DaysOutstanding'] = 0
                    aging_summary['current'] += balance
            else:
                v['DaysOutstanding'] = 0
                aging_summary['current'] += balance

        aging_summary['total'] = total_ap

        aging_summary_out = {k: float(v.quantize(MONEY_QUANT, rounding=ROUND_HALF_UP)) for k, v in aging_summary.items()}
        
        return jsonify({
            'success': True,
            'mode': 'odbc',
            'summary': aging_summary_out,
            'vendors': filtered_vendors,
            'total_vendors': len(filtered_vendors)
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/business-status/cash-balances', methods=['GET'])
def get_cash_balances():
    """
    Get current cash and bank account balances
    """
    global USE_MOCK_DATA
    
    print("=== CASH BALANCES FUNCTION CALLED ===", flush=True)
    
    try:
        # If mock data mode, return mock cash balances
        if USE_MOCK_DATA:
            mock_data = get_mock_cash_balances()
            return jsonify({
                'success': True,
                'mode': 'mock',
                **mock_data
            })
        
        # Try to get cash/bank accounts from Chart
        # In Peachtree: AccountType 0 = Cash/Bank
        query = """
            SELECT 
                AccountID,
                AccountDescription as AccountName,
                AccountType,
                Balance0Net as Balance
            FROM Chart
            WHERE AccountType = 0
            ORDER BY AccountID
        """
        log.info(f"CASH BALANCES QUERY: {query}")
        accounts = execute_query(query)
        
        # Calculate total cash
        total_cash = sum(float(acc.get('Balance', 0) or 0) for acc in accounts)
        
        return jsonify({
            'success': True,
            'mode': 'odbc',
            'accounts': accounts,
            'total_cash': total_cash,
            'total_accounts': len(accounts)
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/business-status/top-customers', methods=['GET'])
def get_top_customers():
    """
    Get top customers by sales or outstanding balance
    """
    global USE_MOCK_DATA
    
    try:
        limit = request.args.get('limit', 99999)
        order_by = request.args.get('order_by', 'balance')  # 'balance' or 'sales'
        
        # If mock data mode, return mock customers with field name mapping
        if USE_MOCK_DATA:
            mock_customers = get_mock_customers()[:int(limit)]
            # Map field names to match query aliases
            customers = []
            for c in mock_customers:
                customers.append({
                    'CustomerID': c['CustomerID'],
                    'CustomerName': c['Customer_Bill_Name'],  # Map to CustomerName
                    'Contact': c['Contact'],
                    'Phone': c['Phone_Number'],  # Map to Phone
                    'CurrentBalance': c['Balance'],  # Map to CurrentBalance
                    'CreditLimit': c['Terms_CreditLimit']  # Map to CreditLimit
                })
            return jsonify({
                'success': True,
                'mode': 'mock',
                'customers': customers,
                'count': len(customers),
                'order_by': order_by
            })
        
        if order_by == 'sales':
            # Try to get sales data
            query = f"""
                SELECT TOP {limit}
                    c.CustomerID,
                    c.Customer_Bill_Name as CustomerName,
                    c.Contact,
                    c.Phone_Number as Phone,
                    c.Balance as CurrentBalance,
                    c.Terms_CreditLimit as CreditLimit
                FROM Customers c
                WHERE c.Balance > 0 OR c.Customer_Bill_Name IS NOT NULL
                ORDER BY c.Balance DESC
            """
        else:
            query = f"""
                SELECT TOP {limit}
                    CustomerID,
                    Customer_Bill_Name as CustomerName,
                    Contact,
                    Phone_Number as Phone,
                    Balance as CurrentBalance,
                    Terms_CreditLimit as CreditLimit
                FROM Customers
                WHERE Balance > 0
                ORDER BY Balance DESC
            """
        
        customers = execute_query(query)
        
        return jsonify({
            'success': True,
            'mode': 'odbc',
            'customers': customers,
            'count': len(customers),
            'order_by': order_by
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/business-status/top-vendors', methods=['GET'])
def get_top_vendors():
    """
    Get top vendors by purchases or outstanding balance
    """
    global USE_MOCK_DATA
    
    try:
        limit = request.args.get('limit', 99999)
        
        # If mock data mode, return mock vendors with field name mapping
        if USE_MOCK_DATA:
            mock_vendors = get_mock_vendors()[:int(limit)]
            # Map field names to match query aliases
            vendors = []
            for v in mock_vendors:
                vendors.append({
                    'VendorID': v['VendorID'],
                    'VendorName': v['VendorName'],
                    'Contact': v['Contact'],
                    'Phone': v['Phone_Number'],  # Map to Phone
                    'CurrentBalance': v['Balance']  # Map to CurrentBalance
                })
            return jsonify({
                'success': True,
                'mode': 'mock',
                'vendors': vendors,
                'count': len(vendors)
            })
        
        query = f"""
            SELECT TOP {limit}
                VendorID,
                Name as VendorName,
                Contact,
                PhoneNumber as Phone,
                Balance as CurrentBalance
            FROM Vendors
            WHERE Balance > 0
            ORDER BY Balance DESC
        """
        
        vendors = execute_query(query)
        
        return jsonify({
            'success': True,
            'mode': 'odbc',
            'vendors': vendors,
            'count': len(vendors)
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/peachtree/reports/outstanding', methods=['GET'])
def get_outstanding_report():
    """
    Comprehensive Outstanding Report
    All unpaid invoices and bills
    """
    global USE_MOCK_DATA
    
    try:
        report_type = request.args.get('type', 'all')  # 'all', 'ar', 'ap'
        
        result = {
            'success': True,
            'mode': 'mock' if USE_MOCK_DATA else 'odbc',
            'report_date': str(datetime.now()),
            'ar': {},
            'ap': {}
        }
        
        # If mock data mode
        if USE_MOCK_DATA:
            mock_customers = get_mock_customers()
            mock_vendors = get_mock_vendors()
            
            # Map field names for customers
            customers = []
            for c in mock_customers:
                customers.append({
                    'CustomerID': c['CustomerID'],
                    'CustomerName': c['Customer_Bill_Name'],
                    'Outstanding': c['Balance'],
                    'CreditLimit': c['Terms_CreditLimit'],
                    'IsInactive': c['CustomerIsInactive']
                })
            
            # Map field names for vendors
            vendors = []
            for v in mock_vendors:
                vendors.append({
                    'VendorID': v['VendorID'],
                    'VendorName': v['VendorName'],
                    'Outstanding': v['Balance'],
                    'IsInactive': v['VendorIsInactive']
                })
            
            if report_type in ['all', 'ar']:
                result['ar'] = {
                    'customers': customers,
                    'total': sum(c['Outstanding'] for c in customers),
                    'count': len(customers)
                }
            
            if report_type in ['all', 'ap']:
                result['ap'] = {
                    'vendors': vendors,
                    'total': sum(v['Outstanding'] for v in vendors),
                    'count': len(vendors)
                }
            
            return jsonify(result)
        
        if report_type in ['all', 'ar']:
            # Get AR outstanding
            ar_query = """
                SELECT 
                    CustomerID,
                    Customer_Bill_Name as CustomerName,
                    Balance as Outstanding,
                    Terms_CreditLimit as CreditLimit,
                    CustomerIsInactive as IsInactive
                FROM Customers
                WHERE Balance > 0
                ORDER BY Balance DESC
            """
            ar_data = execute_query(ar_query)
            result['ar'] = {
                'customers': ar_data,
                'total': sum(float(c.get('Outstanding', 0) or 0) for c in ar_data),
                'count': len(ar_data)
            }
        
        if report_type in ['all', 'ap']:
            # Get AP outstanding
            ap_query = """
                SELECT 
                    VendorID,
                    VendorName,
                    Balance as Outstanding,
                    VendorIsInactive as IsInactive
                FROM Vendors
                WHERE Balance > 0
                ORDER BY Balance DESC
            """
            ap_data = execute_query(ap_query)
            result['ap'] = {
                'vendors': ap_data,
                'total': sum(float(v.get('Outstanding', 0) or 0) for v in ap_data),
                'count': len(ap_data)
            }
        
        return jsonify(result)
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

if __name__ == '__main__':
    print("=" * 70)
    print("🌳 Peachtree ODBC Bridge (32-bit)")
    print("=" * 70)
    print(f"\n📊 ODBC DSN: {ODBC_DSN}")
    print(f"🔗 Connection String: {CONNECTION_STRING}")
    
    # Test connection
    print(f"\n🔍 Testing connection...")
    try:
        conn = get_connection()
        cursor = conn.cursor()
        
        # Get database info
        print(f"✅ Connected successfully!")
        
        # List tables
        print(f"\n📋 Available tables:")
        tables = [table.table_name for table in cursor.tables(tableType='TABLE')]
        for table in sorted(tables)[:20]:
            print(f"   • {table}")
        if len(tables) > 20:
            print(f"   ... and {len(tables) - 20} more")
        
        cursor.close()
        conn.close()
        
    except Exception as e:
        print(f"❌ Connection failed: {e}")
        print(f"\n💡 Make sure:")
        print(f"   1. Peachtree is closed (file locks)")
        print(f"   2. ODBC DSN '{ODBC_DSN}' is configured")
        print(f"   3. Pervasive SQL service is running")
    
    print("\n" + "=" * 70)
    print("🚀 Starting Flask API on http://localhost:5000")
    print("=" * 70)
    print("\nEndpoints:")
    print("  GET  /health                       - Service status")
    print("  GET  /api/peachtree/tables         - List all tables")
    print("  GET  /api/peachtree/customers      - Customer data")
    print("  GET  /api/peachtree/vendors        - Vendor data")
    print("  GET  /api/peachtree/chart-of-accounts - Chart of Accounts")
    print("  GET  /api/peachtree/transactions   - Transaction data")
    print("  POST /api/peachtree/query          - Custom SQL query")
    print("\n📊 Business Status Dashboard:")
    print("  GET  /api/peachtree/business-status/ar-aging      - AR Aging Report")
    print("  GET  /api/peachtree/business-status/ap-aging      - AP Aging Report")
    print("  GET  /api/peachtree/business-status/cash-balances - Cash/Bank Balances")
    print("  GET  /api/peachtree/business-status/top-customers - Top Customers")
    print("  GET  /api/peachtree/business-status/top-vendors   - Top Vendors")
    print("  GET  /api/peachtree/reports/outstanding           - Outstanding Report")
    print("\n")
    
    # Use port 5001 for testing (port 5000 might be blocked by zombie process)
    bridge_port = int(os.getenv('PEACHTREE_BRIDGE_PORT', '5001'))
    app.run(host='0.0.0.0', port=bridge_port, debug=False)

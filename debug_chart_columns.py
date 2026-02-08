
import pyodbc
import json
from decimal import Decimal

def get_connection():
    conn_str = "DSN=Peachtree;UID=Peachtree;PWD=#Snath1;"
    return pyodbc.connect(conn_str, autocommit=True)

def test_query():
    try:
        conn = get_connection()
        cursor = conn.cursor()
        
        # Get columns
        cursor.execute("SELECT TOP 1 * FROM Chart WHERE AccountType = 0")
        columns = [column[0] for column in cursor.description]
        print(f"Columns: {columns}")
        
        row = cursor.fetchone()
        if row:
            data = dict(zip(columns, row))
            # Convert decimals to float for printing
            for k, v in data.items():
                if isinstance(v, Decimal):
                    data[k] = float(v)
            print(json.dumps(data, indent=2, default=str))
        else:
            print("No cash accounts found")
            
        cursor.close()
        conn.close()
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    test_query()

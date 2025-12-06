import { NextRequest, NextResponse } from 'next/server';
import sql from 'mssql';

const sqlConfig = {
    user: process.env.SQL_USER || '',
    password: process.env.SQL_PASSWORD || '',
    server: process.env.SQL_SERVER || '',
    database: process.env.SQL_DATABASE || 'LasanthaTire',
    port: parseInt(process.env.SQL_PORT || '1433') || undefined,
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

export async function POST(req: NextRequest) {
    try {
        const body = await req.json();
        const { refId, selectedItemIndex, name, phone, date, time, notes } = body;

        // Validate required fields
        if (!name || !phone || !date || !time) {
            return NextResponse.json({ 
                success: false, 
                error: 'Name, phone, date, and time are required' 
            }, { status: 400 });
        }

        const pool = await sql.connect(sqlConfig);
        
        // Create appointments table if not exists
        await pool.request().query(`
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
            BEGIN
                CREATE TABLE Appointments (
                    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
                    RefID INT,
                    CustomerName NVARCHAR(100),
                    CustomerPhone NVARCHAR(50),
                    AppointmentDate DATE,
                    AppointmentTime NVARCHAR(20),
                    SelectedItemIndex INT,
                    Notes NVARCHAR(500),
                    Status NVARCHAR(50) DEFAULT 'Pending',
                    CreatedAt DATETIME DEFAULT GETDATE()
                )
            END
        `);

        // Insert appointment
        const result = await pool.request()
            .input('RefID', sql.Int, refId || null)
            .input('CustomerName', sql.NVarChar(100), name)
            .input('CustomerPhone', sql.NVarChar(50), phone)
            .input('AppointmentDate', sql.Date, date)
            .input('AppointmentTime', sql.NVarChar(20), time)
            .input('SelectedItemIndex', sql.Int, selectedItemIndex !== null ? selectedItemIndex : null)
            .input('Notes', sql.NVarChar(500), notes || '')
            .query(`
                INSERT INTO Appointments (RefID, CustomerName, CustomerPhone, AppointmentDate, AppointmentTime, SelectedItemIndex, Notes)
                OUTPUT INSERTED.AppointmentID
                VALUES (@RefID, @CustomerName, @CustomerPhone, @AppointmentDate, @AppointmentTime, @SelectedItemIndex, @Notes)
            `);

        const appointmentId = result.recordset[0].AppointmentID;

        return NextResponse.json({ 
            success: true, 
            appointmentId,
            message: 'Appointment booked successfully'
        });

    } catch (error: unknown) {
        console.error('Error booking appointment:', error);
        const errorMessage = error instanceof Error ? error.message : 'Internal server error';
        return NextResponse.json({ 
            success: false, 
            error: errorMessage
        }, { status: 500 });
    }
}

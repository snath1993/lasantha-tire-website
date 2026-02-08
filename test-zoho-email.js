const nodemailer = require('nodemailer');

async function testZohoEmail() {
    console.log('🔍 Testing Zoho Email Connection...\n');
    
    const config = {
        host: 'smtp.zoho.com',
        port: 465,
        secure: true,
        auth: {
            user: 'info@lasanthatyre.com',
            pass: 'RAWnP5ZhfZtt'
        },
        debug: true,
        logger: true
    };
    
    console.log('📧 Config:', {
        host: config.host,
        port: config.port,
        user: config.auth.user,
        pass: '***' + config.auth.pass.slice(-4)
    });
    
    try {
        const transporter = nodemailer.createTransport(config);
        
        console.log('\n🔄 Verifying connection...');
        await transporter.verify();
        
        console.log('\n✅ CONNECTION SUCCESSFUL!');
        console.log('Zoho SMTP is working correctly.\n');
        
        // Send test email
        console.log('📤 Sending test email...');
        const info = await transporter.sendMail({
            from: '"Lasantha Tyre Traders" <info@lasanthatyre.com>',
            to: 'info@lasanthatyre.com',
            subject: 'Test Email from WhatsApp Bot',
            text: 'This is a test email to verify Zoho integration is working!',
            html: '<h2>✅ Success!</h2><p>Zoho email integration is working correctly.</p>'
        });
        
        console.log('\n✅ EMAIL SENT!');
        console.log('Message ID:', info.messageId);
        console.log('Response:', info.response);
        
    } catch (error) {
        console.error('\n❌ ERROR:', error.message);
        if (error.code) console.error('Error Code:', error.code);
        if (error.command) console.error('Failed Command:', error.command);
        if (error.responseCode) console.error('Response Code:', error.responseCode);
    }
}

testZohoEmail();

const fs = require('fs');
const path = require('path');

// Read the captions file
const captionsFile = path.join(__dirname, '..', 'fallback-captions.json');
const rawData = fs.readFileSync(captionsFile, 'utf-8');
const data = JSON.parse(rawData);

let htmlContent = `
<!DOCTYPE html>
<html lang="si">
<head>
    <meta charset="UTF-8">
    <title>Lasantha Tyre Traders - Caption Collection</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Noto+Sans+Sinhala:wght@400;700&display=swap');
        
        body {
            font-family: 'Noto Sans Sinhala', Helvetica, Arial, sans-serif;
            background-color: #f0f2f5;
            padding: 20px;
            color: #1c1e21;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
        }
        .header {
            text-align: center;
            margin-bottom: 30px;
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 1px 2px rgba(0,0,0,0.1);
        }
        .post-card {
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 1px 2px rgba(0,0,0,0.2);
            margin-bottom: 20px;
            padding: 15px;
            page-break-inside: avoid;
        }
        .post-header {
            display: flex;
            align-items: center;
            margin-bottom: 12px;
        }
        .avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background-color: #ddd;
            margin-right: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            color: #555;
        }
        .user-info h4 {
            margin: 0;
            font-size: 15px;
            color: #050505;
        }
        .user-info span {
            font-size: 13px;
            color: #65676b;
        }
        .post-content {
            white-space: pre-wrap;
            font-size: 15px;
            line-height: 1.5;
        }
        .post-footer {
            border-top: 1px solid #ced0d4;
            margin-top: 10px;
            padding-top: 10px;
            color: #65676b;
            font-size: 13px;
        }
        
        @media print {
            body { background: white; }
            .post-card { border: 1px solid #ddd; box-shadow: none; }
            .no-print { display: none; }
        }
        
        .btn {
            background: #1877f2;
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 6px;
            font-size: 16px;
            cursor: pointer;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="no-print" style="text-align: center;">
            <button class="btn" onclick="window.print()">Save as PDF / Print</button>
            <p>PDF එකක් ලෙස save කරගැනීමට උඩ ඇති බොත්තම ඔබා "Save as PDF" තෝරන්න.</p>
        </div>

        <div class="header">
            <h1>Lasantha Tyre Traders</h1>
            <h2>Caption Collection (Total: ${data.captions.length})</h2>
            <p>Generated on: ${new Date().toLocaleDateString()}</p>
        </div>

        ${data.captions.map((caption, index) => `
        <div class="post-card">
            <div class="post-header">
                <div class="avatar">LT</div>
                <div class="user-info">
                    <h4>Lasantha Tyre Traders</h4>
                    <span>Caption #${index + 1}</span>
                </div>
            </div>
            <div class="post-content">${caption}</div>
        </div>
        `).join('')}
    </div>
</body>
</html>
`;

const outputPath = path.join(__dirname, '..', 'all_captions.html');
fs.writeFileSync(outputPath, htmlContent, 'utf-8');
console.log('HTML file created at:', outputPath);

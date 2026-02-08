// utils/monthlyReportMessageHandler.js
// Handler for parsing WhatsApp messages requesting monthly sales reports

const moment = require('moment');
const path = require('path');
const fs = require('fs');

// Function to parse month/year from message
function parseMonthYearFromMessage(messageText) {
    // Look for patterns like "Sales report 2025/05", "2025/05", "05/2025", "May 2025", etc.
    const patterns = [
        /sales\s+report\s+(\d{4})\/(\d{1,2})/i,
        /(\d{4})\/(\d{1,2})/,
        /(\d{1,2})\/(\d{4})/,
        /(\w+)\s+(\d{4})/i
    ];
    
    for (const pattern of patterns) {
        const match = messageText.match(pattern);
        if (match) {
            if (pattern.source.includes('(\\w+)')) {
                // Month name pattern
                const monthName = match[1];
                const year = parseInt(match[2]);
                const month = moment(monthName, 'MMMM').month() + 1; // moment months are 0-indexed
                if (month > 0 && month <= 12) {
                    return { year, month };
                }
            } else if (pattern.source.includes('(\\d{1,2})\\/(\\d{4})')) {
                // MM/YYYY pattern
                const month = parseInt(match[1]);
                const year = parseInt(match[2]);
                if (month >= 1 && month <= 12) {
                    return { year, month };
                }
            } else {
                // YYYY/MM pattern
                const year = parseInt(match[1]);
                const month = parseInt(match[2]);
                if (month >= 1 && month <= 12) {
                    return { year, month };
                }
            }
        }
    }
    
    return null;
}

// Function to check if message is a monthly report request
function isMonthlyReportRequest(messageText, fromNumber) {
    const adminNumbers = ['0771222509', '0777311770'];
    
    // Check if message is from admin numbers (normalize numbers)
    const normalizedFrom = fromNumber.replace(/^94/, '0').replace(/^\+94/, '0');
    const isFromAdmin = adminNumbers.includes(normalizedFrom);
    if (!isFromAdmin) return false;
    
    // Check if message contains sales report keywords
    const keywords = ['sales report', 'monthly report', 'report'];
    const hasKeyword = keywords.some(keyword => 
        messageText.toLowerCase().includes(keyword.toLowerCase())
    );
    
    if (!hasKeyword) return false;
    
    // Try to parse month/year
    const parsed = parseMonthYearFromMessage(messageText);
    return parsed !== null;
}

// Function to generate month range for given Date object or year/month
function getMonthRange(dateOrYear, month = null) {
    let targetDate;
    
    if (month !== null) {
        // Called with year, month parameters
        targetDate = moment({ year: dateOrYear, month: month - 1, day: 1 });
    } else {
        // Called with Date object
        targetDate = moment(dateOrYear);
    }
    
    const startDate = targetDate.clone().startOf('month').toDate();
    const endDate = targetDate.clone().endOf('month').toDate();
    const start = targetDate.clone().startOf('month').format('YYYY-MM-DD');
    const end = targetDate.clone().endOf('month').format('YYYY-MM-DD');
    
    return { 
        startDate, 
        endDate, 
        start, 
        end, 
        monthName: targetDate.format('MMMM YYYY') 
    };
}

module.exports = {
    parseMonthYearFromMessage,
    isMonthlyReportRequest,
    getMonthRange
};
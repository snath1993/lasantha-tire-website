// utils/jobsConfigReader.js
// Utility to read job configurations and contact numbers
const fs = require('fs');
const path = require('path');

const JOBS_CONFIG_FILE = path.join(__dirname, '..', 'config', 'jobs.json');

function loadJobsConfig() {
    try {
        if (!fs.existsSync(JOBS_CONFIG_FILE)) {
            return null;
        }
        const data = fs.readFileSync(JOBS_CONFIG_FILE, 'utf-8');
        return JSON.parse(data);
    } catch (error) {
        console.error('Error loading jobs config:', error.message);
        return null;
    }
}

function getJobContactNumbers(jobId) {
    const config = loadJobsConfig();
    if (!config || !config[jobId]) {
        // Fallback to environment variables
        return getDefaultContactNumbers();
    }
    
    const jobConfig = config[jobId];
    if (!jobConfig.enabled) {
        return []; // Return empty array if job is disabled
    }
    
    return jobConfig.contactNumbers || getDefaultContactNumbers();
}

function isJobAllowEveryone(jobId) {
    const config = loadJobsConfig();
    if (!config || !config[jobId]) {
        return false;
    }

    const jobConfig = config[jobId];
    if (typeof jobConfig.allowEveryone === 'boolean') {
        return jobConfig.allowEveryone;
    }

    if (Array.isArray(jobConfig.contactNumbers) && jobConfig.contactNumbers.length > 0) {
        return false;
    }

    return true;
}

function getDefaultContactNumbers() {
    const numbers = process.env.REPORT_NUMBERS || process.env.DAILY_REPORT_NUMBERS || '';
    return numbers.split(',').map(n => n.trim()).filter(Boolean);
}

function isJobEnabled(jobId) {
    const config = loadJobsConfig();
    if (!config || !config[jobId]) {
        return true; // Default to enabled if no config
    }
    return config[jobId].enabled !== false;
}

function getJobConfig(jobId) {
    const config = loadJobsConfig();
    if (!config || !config[jobId]) {
        return {
            name: jobId,
            description: 'No description available',
            contactNumbers: getDefaultContactNumbers(),
            enabled: true
        };
    }
    return config[jobId];
}

function getAllJobsConfig() {
    return loadJobsConfig() || {};
}

module.exports = {
    loadJobsConfig,
    getJobContactNumbers,
    getDefaultContactNumbers,
    isJobEnabled,
    getJobConfig,
    getAllJobsConfig,
    isJobAllowEveryone
};
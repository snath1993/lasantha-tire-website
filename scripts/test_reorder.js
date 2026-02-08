const ReOrderingJob = require('../jobs/ReOrderingJob');
const { getPool } = require('../utils/sqlPool');
const { aiPoolConnect, aiPool } = require('../utils/aiDbConnection');

// Mock Client
const client = {
    getChats: async () => [],
    sendMessage: async (to, msg) => console.log(`[MOCK SEND] To: ${to}\nMessage: ${msg}`)
};

// Mock Log
const logAndSave = (msg) => console.log(`[LOG] ${msg}`);

async function run() {
    try {
        console.log('Connecting to DB...');
        const mainPool = await getPool();
        await aiPoolConnect;
        
        console.log('Running ReOrderingJob...');
        await ReOrderingJob(client, mainPool, aiPool, [], logAndSave);
        
        console.log('Done.');
        process.exit(0);
    } catch (e) {
        console.error('Job Failed:', e);
        process.exit(1);
    }
}

run();

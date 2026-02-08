// =====================================================
// Deletion Queue Processor Job
// Purpose: Process pending invoice deletions from queue
// Runs: Every 1 minute
// =====================================================

const moment = require('moment');
const InvoiceDeletionJob = require('./invoiceDeletionJob');
const { updateJobStatus } = require('../utils/jobStatus');
const { aiPoolConnect, aiPool } = require('../utils/aiDbConnection');

module.exports = async function DeletionQueueProcessorJob(sql, sqlConfig, sendWhatsAppMessage, logAndSave, options = {}) {
    const jobName = 'DeletionQueueProcessorJob';
    const started = new Date();
    let pool = null;

    try {
        // Silent processing - only log when there's actual work to do

        // Ensure WhatsAppAI pool is connected (shared pool managed by aiDbConnection)
        await aiPoolConnect;
        pool = aiPool;
        if (!pool || pool.connected !== true) {
            const error = `${jobName}: Job error - WhatsAppAI pool not available`;
            logAndSave(error);
            throw new Error(error);
        }

        // Get pending deletions that are ready to process
        const now = new Date();

        const queueRequest = pool.request();
        queueRequest.input('now', sql.DateTime, now);
        const result = await queueRequest.query`
            SELECT 
                QueueID, InvoiceNo, ItemID, ItemDescription,
                SoldDate, ScheduledDeletionTime, Pattern, Qty, RetryCount
            FROM WhatsAppAI.dbo.TblDeletionQueue
            WHERE Status = 'PENDING'
            AND ScheduledDeletionTime <= ${now}
            ORDER BY ScheduledDeletionTime ASC
        `;

        const pendingItems = result.recordset || [];

        if (pendingItems.length === 0) {
            // Silent - no logging when queue is empty
            await updateJobStatus(jobName, {
                lastRun: started.toISOString(),
                lastSuccess: true,
                lastError: null,
                processedCount: 0,
                pendingCount: 0
            });
            return;
        }
        
        // Only log when there's actual work
        logAndSave(`${jobName}: Starting queue processing - Found ${pendingItems.length} pending deletion(s)`);

        let successCount = 0;
        let failCount = 0;

        // Process each pending deletion
        for (const item of pendingItems) {
            try {
                logAndSave(`${jobName}: Processing QueueID ${item.QueueID} - Invoice ${item.InvoiceNo}`);

                // Mark as PROCESSING
                await pool.request().query`
                    UPDATE WhatsAppAI.dbo.TblDeletionQueue
                    SET Status = 'PROCESSING'
                    WHERE QueueID = ${item.QueueID}
                `;

                // Initialize deletion job
                const deletionJob = new InvoiceDeletionJob();
                await deletionJob.initialize();

                // Process the deletion
                const reason = `Watched item sold - Pattern: ${item.Pattern || 'N/A'}, Qty: ${item.Qty || 0}`;
                const result = await deletionJob.processInvoiceDeletion(
                    item.InvoiceNo,
                    item.ItemID || 'Unknown',
                    reason
                );

                // Close deletion job connections
                await deletionJob.close();

                if (result.success) {
                    // Mark as COMPLETED
                    await pool.request().query`
                        UPDATE WhatsAppAI.dbo.TblDeletionQueue
                        SET 
                            Status = 'COMPLETED',
                            ProcessedDate = ${now},
                            ProcessedBy = ${jobName}
                        WHERE QueueID = ${item.QueueID}
                    `;

                    logAndSave(`${jobName}: ✅ Successfully deleted invoice ${item.InvoiceNo}`);
                    successCount++;

                    // Send WhatsApp notification to admins
                    try {
                        const notificationAdmins = ['0771222509', '0777311770', '0777078700'];
                        const notifyMsg = `✅ *Invoice Auto-Voided*\n\n` +
                            `📋 Invoice: ${item.InvoiceNo}\n` +
                            `📦 Item: ${item.ItemDescription || item.ItemID}\n` +
                            `🏷️ Pattern: ${item.Pattern || 'N/A'}\n` +
                            `📊 Qty: ${item.Qty || 0}\n\n` +
                            `🕐 Sold: ${moment(item.SoldDate).format('YYYY-MM-DD HH:mm')}\n` +
                            `� Voided: ${moment(now).format('YYYY-MM-DD HH:mm')}\n\n` +
                            `💾 Backup: WhatsAppAI.dbo.TblDelBack\n` +
                            `� Action: ${result.rowsDeleted} rows deleted, void placeholder created\n` +
                            `🔢 Counter: NOT MODIFIED (no gaps!)\n\n` +
                            `Status: SUCCESS ✅`;

                        for (const admin of notificationAdmins) {
                            try {
                                await sendWhatsAppMessage(admin, notifyMsg);
                                // Add delay to prevent rate limiting
                                await new Promise(resolve => setTimeout(resolve, 2000));
                            } catch (msgErr) {
                                logAndSave(`${jobName}: Failed to notify ${admin}: ${msgErr.message}`);
                            }
                        }
                    } catch (notifyErr) {
                        logAndSave(`${jobName}: Notification error: ${notifyErr.message}`);
                    }

                } else {
                    throw new Error('Deletion returned unsuccessful result');
                }

            } catch (error) {
                // Mark as FAILED and increment retry count
                const newRetryCount = (item.RetryCount || 0) + 1;
                const maxRetries = 3;

                if (newRetryCount >= maxRetries) {
                    // Max retries reached - mark as FAILED permanently
                    await pool.request().query`
                        UPDATE WhatsAppAI.dbo.TblDeletionQueue
                        SET 
                            Status = 'FAILED',
                            ProcessedDate = ${now},
                            ProcessedBy = ${jobName},
                            ErrorMessage = ${error.message.substring(0, 1000)},
                            RetryCount = ${newRetryCount}
                        WHERE QueueID = ${item.QueueID}
                    `;
                    logAndSave(`${jobName}: ❌ Failed invoice ${item.InvoiceNo} after ${maxRetries} retries: ${error.message}`);
                    
                    // Send failure notification to admins
                    try {
                        const failureAdmins = ['0771222509', '0777311770', '0777078700'];
                        const failMsg = `❌ *Invoice Deletion FAILED*\n\n` +
                            `📋 Invoice: ${item.InvoiceNo}\n` +
                            `📦 Item: ${item.ItemDescription || item.ItemID}\n` +
                            `🏷️ Pattern: ${item.Pattern || 'N/A'}\n\n` +
                            `⚠️ Error: ${error.message.substring(0, 200)}\n` +
                            `🔄 Retries: ${newRetryCount}/${maxRetries}\n\n` +
                            `Status: PERMANENT FAILURE ❌\n` +
                            `Action: Manual review required`;

                        for (const admin of failureAdmins) {
                            try {
                                await sendWhatsAppMessage(admin, failMsg);
                                // Add delay to prevent rate limiting
                                await new Promise(resolve => setTimeout(resolve, 2000));
                            } catch (msgErr) {
                                logAndSave(`${jobName}: Failed to notify failure to ${admin}: ${msgErr.message}`);
                            }
                        }
                    } catch (failNotifyErr) {
                        logAndSave(`${jobName}: Failure notification error: ${failNotifyErr.message}`);
                    }
                } else {
                    // Reset to PENDING for retry, schedule for 1 minute later
                    const nextRetry = new Date(Date.now() + 60000);
                    await pool.request().query`
                        UPDATE WhatsAppAI.dbo.TblDeletionQueue
                        SET 
                            Status = 'PENDING',
                            ScheduledDeletionTime = ${nextRetry},
                            ErrorMessage = ${error.message.substring(0, 1000)},
                            RetryCount = ${newRetryCount}
                        WHERE QueueID = ${item.QueueID}
                    `;
                    logAndSave(`${jobName}: ⚠️  Failed invoice ${item.InvoiceNo}, will retry (${newRetryCount}/${maxRetries}): ${error.message}`);
                }

                failCount++;
            }
        }

        // Get remaining pending count
        const remainingResult = await pool.request().query`
            SELECT COUNT(*) as PendingCount
            FROM WhatsAppAI.dbo.TblDeletionQueue
            WHERE Status = 'PENDING'
        `;
        const remainingCount = remainingResult.recordset[0].PendingCount;

        logAndSave(`${jobName}: Completed - Success: ${successCount}, Failed: ${failCount}, Remaining: ${remainingCount}`);

        // Send summary report to specified admin numbers
        try {
            const reportAdmins = ['0771222509', '0777311770', '0777078700'];
            const statusEmoji = failCount > 0 ? '⚠️' : '✅';
            const summaryMsg = `${statusEmoji} *Invoice Deletion Job Report*\n\n` +
                `⏰ Completed: ${moment(new Date()).format('YYYY-MM-DD HH:mm:ss')}\n\n` +
                `📊 Summary:\n` +
                `✅ Success: ${successCount}\n` +
                `❌ Failed: ${failCount}\n` +
                `⏳ Pending: ${remainingCount}\n\n` +
                `Status: ${failCount === 0 ? 'All OK ✅' : 'Some Failed ⚠️'}`;

            for (const admin of reportAdmins) {
                try {
                    await sendWhatsAppMessage(admin, summaryMsg);
                    logAndSave(`${jobName}: Summary sent to ${admin}`);
                    // Add delay to prevent rate limiting
                    await new Promise(resolve => setTimeout(resolve, 2000));
                } catch (msgErr) {
                    logAndSave(`${jobName}: Failed to send summary to ${admin}: ${msgErr.message}`);
                }
            }
        } catch (reportErr) {
            logAndSave(`${jobName}: Summary report error: ${reportErr.message}`);
        }

        await updateJobStatus(jobName, {
            lastRun: started.toISOString(),
            lastSuccess: true,
            lastError: null,
            processedCount: successCount,
            failedCount: failCount,
            pendingCount: remainingCount
        });

    } catch (error) {
        logAndSave(`${jobName}: Job error - ${error.message}`);
        await updateJobStatus(jobName, {
            lastRun: started.toISOString(),
            lastSuccess: false,
            lastError: error.message
        });
    } finally {
        // Shared pool handled by aiDbConnection; no close action here
    }
};

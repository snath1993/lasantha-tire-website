import { Request, Response } from 'express';
import { WhatsAppService } from '../services/whatsappService';
import { DatabaseService } from '../services/databaseService';

class WhatsAppController {
    constructor(private readonly whatsappService: WhatsAppService, private readonly databaseService: DatabaseService) {}

    async receiveMessage(req: Request, res: Response): Promise<void> {
        const messageData = req.body;
        await this.whatsappService.receiveWhatsAppMessage(messageData);
        res.sendStatus(200);
    }

    async sendMessage(req: Request, res: Response): Promise<void> {
        const { messageId } = req.body as { messageId?: number };

        if (typeof messageId !== 'number') {
            res.status(400).send('Invalid messageId');
            return;
        }

        const messageData = await this.databaseService.fetchMessageData(messageId);

        if (!messageData) {
            res.status(404).send('Message not found');
            return;
        }

        await this.whatsappService.sendWhatsAppMessage(messageData.sender, messageData.content);
        res.sendStatus(200);
    }

    async getMessages(req: Request, res: Response): Promise<void> {
        try {
            const limit = parseInt(req.query.limit as string) || 50;
            const messages = await this.databaseService.listRecentMessages(limit);
            res.json(messages);
        } catch (error) {
            console.error('Error fetching messages:', error);
            res.status(500).send('Failed to fetch messages');
        }
    }
}

export default WhatsAppController;
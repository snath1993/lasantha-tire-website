import { Application, Router } from 'express';
import WhatsAppController from '../controllers/whatsappController';
import { WhatsAppService } from '../services/whatsappService';
import { DatabaseService } from '../services/databaseService';

const router = Router();

const whatsappService = new WhatsAppService(process.env.WHATSAPP_API_URL ?? '');
const databaseService = new DatabaseService();
const whatsappController = new WhatsAppController(whatsappService, databaseService);

router.post('/receive', whatsappController.receiveMessage.bind(whatsappController));
router.post('/send', whatsappController.sendMessage.bind(whatsappController));
router.get('/messages', whatsappController.getMessages.bind(whatsappController));

export function setRoutes(app: Application): void {
    app.use('/whatsapp', router);
}

export default router;
import axios from 'axios';

export class WhatsAppService {
    private readonly apiUrl: string;

    constructor(apiUrl: string) {
        this.apiUrl = apiUrl;
    }

    public async sendWhatsAppMessage(to: string, message: string): Promise<void> {
        try {
            await axios.post(`${this.apiUrl}/send`, {
                to,
                message
            });
            
            // Emit outbound message event
            const io = (global as any).io;
            if (io) {
                io.emit('whatsapp:outbound', {
                    to,
                    message,
                    timestamp: new Date().toISOString()
                });
            }
        } catch (error) {
            console.error('Error sending message:', error);
            throw new Error('Failed to send message');
        }
    }

    public async receiveWhatsAppMessage(messageData?: unknown): Promise<any> {
        try {
            let data;
            if (messageData) {
                data = messageData;
            } else {
                const response = await axios.get(`${this.apiUrl}/receive`);
                data = response.data;
            }
            
            // Emit inbound message event
            const io = (global as any).io;
            if (io && data) {
                io.emit('whatsapp:inbound', {
                    ...data,
                    timestamp: new Date().toISOString()
                });
            }
            
            return data;
        } catch (error) {
            console.error('Error receiving message:', error);
            throw new Error('Failed to receive message');
        }
    }
}
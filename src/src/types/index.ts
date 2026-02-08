export interface Message {
    id: number;
    content: string;
    sender: string;
    timestamp: Date;
}

export interface WhatsAppResponse {
    status: string;
    message: string;
}
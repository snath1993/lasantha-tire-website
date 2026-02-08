import express from 'express';
import bodyParser from 'body-parser';
import { createServer } from 'http';
import { Server as SocketIOServer } from 'socket.io';
import { setRoutes } from './routes/whatsappRoutes';

const app = express();
const server = createServer(app);
const io = new SocketIOServer(server, {
    cors: {
        origin: process.env.CLIENT_ORIGIN || "http://localhost:3000",
        methods: ["GET", "POST"]
    }
});
const PORT = process.env.PORT || 3000;

// Make io available globally for services
(global as any).io = io;

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

setRoutes(app);

io.on('connection', (socket) => {
    console.log('Client connected:', socket.id);
    
    socket.on('disconnect', () => {
        console.log('Client disconnected:', socket.id);
    });
});

server.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
    console.log(`Socket.IO server is ready`);
});
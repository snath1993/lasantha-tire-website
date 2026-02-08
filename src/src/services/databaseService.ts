import { Pool, PoolConfig } from 'pg';
import { Message } from '../models/messageModel';

export class DatabaseService {
    private readonly pool: Pool;

    constructor(config?: PoolConfig) {
        const connection: PoolConfig = config ?? {
            user: process.env.DB_USER ?? 'postgres',
            host: process.env.DB_HOST ?? 'localhost',
            database: process.env.DB_NAME ?? 'postgres',
            password: process.env.DB_PASSWORD ?? 'postgres',
            port: Number(process.env.DB_PORT ?? 5432)
        };

        this.pool = new Pool(connection);
    }

    async fetchMessageData(id: number): Promise<Message | null> {
        const query = 'SELECT * FROM messages WHERE id = $1';
        const result = await this.pool.query<Message>(query, [id]);
        return result.rows.length > 0 ? result.rows[0] : null;
    }

    async saveMessageData(message: Message): Promise<void> {
        const query = 'INSERT INTO messages(content, sender, timestamp) VALUES($1, $2, $3)';
        await this.pool.query(query, [message.content, message.sender, message.timestamp]);
    }

    async listRecentMessages(limit: number = 50): Promise<Message[]> {
        const query = 'SELECT * FROM messages ORDER BY timestamp DESC LIMIT $1';
        const result = await this.pool.query<Message>(query, [limit]);
        return result.rows;
    }
}
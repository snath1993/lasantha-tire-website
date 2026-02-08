import { SampleData } from '../types';
import { Database } from '../database';

export class SampleService {
    private readonly db: Database;

    constructor() {
        this.db = new Database();
    }

    public async fetchSampleData(): Promise<SampleData[]> {
        const data = await this.db.query('SELECT * FROM sample_table');
        return data;
    }

    public async processSampleData(data: SampleData): Promise<void> {
        await this.db.insert('sample_table', data);
    }
}
import { SampleData } from './types';

export class Database {
    private readonly memoryStore: SampleData[] = [];

    public async query(_sql: string): Promise<SampleData[]> {
        return this.memoryStore;
    }

    public async insert(_table: string, data: SampleData): Promise<void> {
        this.memoryStore.push(data);
    }
}

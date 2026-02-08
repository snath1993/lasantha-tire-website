import { Request, Response } from 'express';

export class IndexController {
    public getSampleData(req: Request, res: Response): void {
        const sampleData = {
            message: "This is sample data",
            timestamp: new Date()
        };
        res.json(sampleData);
    }
}
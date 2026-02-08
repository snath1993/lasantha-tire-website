export interface QuoteQueueMeta {
    name?: string;
    originalPhone?: string;
    tyreSize?: string;
    [key: string]: unknown;
}

export interface QuoteQueueEntry {
    id: string;
    number: string;
    message: string;
    source: string;
    meta: QuoteQueueMeta;
    createdAt: string;
    attempts: number;
    lastAttemptAt: string | null;
    lastError: string | null;
}

export declare function getQueueFilePath(): string;
export declare function ensureQueueFile(): Promise<void>;
export declare function getQueuedQuoteRequests(): Promise<QuoteQueueEntry[]>;
export declare function enqueueQuoteRequest(data: {
    number: string;
    message: string;
    source?: string;
    meta?: QuoteQueueMeta;
    attempts?: number;
}): Promise<QuoteQueueEntry>;
export declare function replaceQueuedQuoteRequests(entries: QuoteQueueEntry[]): Promise<QuoteQueueEntry[]>;
export declare function upsertQuoteRequest(entry: QuoteQueueEntry): Promise<QuoteQueueEntry>;

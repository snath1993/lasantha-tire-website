import fs from 'fs/promises';
import path from 'path';

interface AppConfig {
  apiKeys: {
    openai: string;
    facebook: string;
    gemini: string;
  };
  features: {
    aiEnabled: boolean;
    jobsEnabled: boolean;
  };
  adminNumbers: string[];
}

class ConfigService {
  private static instance: ConfigService;
  private config: AppConfig | null = null;
  private configPath = path.join(process.cwd(), 'config', 'settings.json');

  private constructor() {}

  public static getInstance(): ConfigService {
    if (!ConfigService.instance) {
      ConfigService.instance = new ConfigService();
    }
    return ConfigService.instance;
  }

  public async load(): Promise<AppConfig> {
    if (!this.config) {
      try {
        const data = await fs.readFile(this.configPath, 'utf-8');
        // Remove BOM if present (fixes Windows encoding issues)
        const cleanData = data.replace(/^\uFEFF/, '');
        this.config = JSON.parse(cleanData);
        console.log(' Configuration loaded');
      } catch (error) {
        console.error('Failed to load config:', error);
        this.config = this.getDefaultConfig();
      }
    }
    return this.config!; // Non-null assertion: always set in try or catch
  }

  public async save(config: AppConfig): Promise<void> {
    await fs.writeFile(this.configPath, JSON.stringify(config, null, 2), 'utf-8');
    this.config = config;
    console.log(' Configuration saved');
  }

  public get(): AppConfig {
    if (!this.config) {
      throw new Error('Config not loaded. Call load() first.');
    }
    return this.config;
  }

  private getDefaultConfig(): AppConfig {
    return {
      apiKeys: {
        openai: '',
        facebook: '',
        gemini: '',
      },
      features: {
        aiEnabled: true,
        jobsEnabled: true,
      },
      adminNumbers: [],
    };
  }
}

export const configService = ConfigService.getInstance();
export type { AppConfig };

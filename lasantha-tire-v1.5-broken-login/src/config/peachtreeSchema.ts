import schema from './peachtree-schema.json';

type RelationshipType = '1:1' | '1:M' | 'M:1' | 'M:M' | string;

export interface PeachtreeRelationship {
  to: string;
  type: RelationshipType;
  localKey: string;
  foreignKey: string;
}

export interface PeachtreeTableDefinition {
  name: string;
  module: string;
  description: string;
  primaryKey: string;
  businessKey?: string;
  fields: string[];
  relationships: PeachtreeRelationship[];
}

export interface PeachtreeSchemaDefinition {
  version: string;
  source: string;
  notes: string[];
  tables: PeachtreeTableDefinition[];
}

const PEACHTREE_SCHEMA = schema as PeachtreeSchemaDefinition;
const TABLE_MAP = new Map(
  PEACHTREE_SCHEMA.tables.map((table) => [table.name.toLowerCase(), table])
);

export const listModules = (): string[] => {
  return Array.from(new Set(PEACHTREE_SCHEMA.tables.map((table) => table.module)));
};

export const listTablesByModule = (module?: string): PeachtreeTableDefinition[] => {
  if (!module) return PEACHTREE_SCHEMA.tables;
  return PEACHTREE_SCHEMA.tables.filter((table) => table.module === module);
};

export const getTableDefinition = (tableName: string): PeachtreeTableDefinition | undefined => {
  return TABLE_MAP.get(tableName.toLowerCase());
};

export const getPeachtreeSchema = (): PeachtreeSchemaDefinition => PEACHTREE_SCHEMA;
export const getSchemaNotes = (): string[] => PEACHTREE_SCHEMA.notes;

export type { PeachtreeSchemaDefinition as PeachtreeSchema };

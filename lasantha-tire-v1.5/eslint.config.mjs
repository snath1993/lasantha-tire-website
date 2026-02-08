import { defineConfig, globalIgnores } from "eslint/config";
import nextVitals from "eslint-config-next/core-web-vitals";
import nextTs from "eslint-config-next/typescript";

const eslintConfig = defineConfig([
  ...nextVitals,
  ...nextTs,
  // Override default ignores of eslint-config-next.
  globalIgnores([
    // Default ignores of eslint-config-next:
    ".next/**",
    "out/**",
    "build/**",
    "next-env.d.ts",

    // Local scripts / scratch files (not shipped).
    "scripts/**",
    "check_columns*.js",
    "test-*.js",
    "test-*.ts",
  ]),

  // Project overrides: keep lint actionable without blocking on legacy typing debt.
  {
    files: ["**/*.{js,jsx,ts,tsx}"],
    rules: {
      "@typescript-eslint/no-explicit-any": "warn",
      "react/no-unescaped-entities": "warn",
      "react-hooks/set-state-in-effect": "warn",
      "prefer-const": "warn",
      "@typescript-eslint/no-unused-expressions": "warn",
    },
  },

  {
    files: ["src/app/api/**/route.{ts,tsx}"],
    rules: {
      "@typescript-eslint/no-require-imports": "off",
    },
  },
]);

export default eslintConfig;

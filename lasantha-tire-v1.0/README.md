# Lasantha Tire v1.0 - Advanced Dashboard

This is the new, production-ready dashboard for Lasantha Tire, built with Next.js 16, Tailwind CSS, and Shadcn UI concepts.

## Features

- **Modern UI/UX**: Clean, responsive design with a sidebar layout.
- **Sales Analytics**: Interactive charts and PDF export functionality.
- **Job Manager**: Manual control over system jobs (Backup, Reports, etc.).
- **AI History**: View past conversations between the AI bot and customers.
- **Real-time Stats**: Dashboard overview with key performance indicators.

## Getting Started

1.  **Install Dependencies**:
    ```bash
    npm install
    ```

2.  **Run Development Server**:
    ```bash
    npm run dev
    ```
    The dashboard will be available at `http://localhost:3000`.

## Project Structure

- `src/app`: App Router pages.
- `src/components/ui`: Reusable UI components (Buttons, Cards).
- `src/components/layout`: Sidebar and Header.
- `src/lib`: Utility functions.

## Integration

- **Backend**: Connects to the existing Node.js bot on port 8585.
- **Database**: Designed to connect to the existing SQL Server (configuration needed in `src/lib/db.ts` for live data).

## Deployment

Build the application for production:
```bash
npm run build
npm start
```

/** @type {import('next').NextConfig} */
const nextConfig = {
  env: {
    WHATSAPP_BOT_URL: process.env.WHATSAPP_BOT_URL || 'http://localhost:8585',
  },
};

export default nextConfig;

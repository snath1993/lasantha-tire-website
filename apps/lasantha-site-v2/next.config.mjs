/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'upload.wikimedia.org',
      },
      {
        protocol: 'https',
        hostname: 'pngimg.com',
      },
      {
        protocol: 'https',
        hostname: 'purepng.com',
      },
      {
        protocol: 'https',
        hostname: 'www.pngall.com',
      },
      {
        protocol: 'https',
        hostname: 'placehold.co',
      }
    ],
    unoptimized: false,
    dangerouslyAllowSVG: true,
    contentSecurityPolicy: "default-src 'self'; script-src 'none'; sandbox;",
  },
  typescript: {
    ignoreBuildErrors: false,
  },
  eslint: {
    ignoreDuringBuilds: false,
  },
}

export default nextConfig

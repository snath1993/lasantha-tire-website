FROM node:18-slim

# Install Google Chrome Stable and fonts
# Note: We need this because the bot runs a browser inside the container
RUN apt-get update && apt-get install -y wget gnupg \
    && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install -y google-chrome-stable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf libxss1 \
      --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*

# Set Environment Variables
ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD=true
ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/google-chrome-stable
ENV CHROMIUM_PATH=/usr/bin/google-chrome-stable

# Create App Directory
WORKDIR /usr/src/app

# Copy Package Files
COPY package*.json ./

# Install Dependencies
RUN npm install

# Copy Project Files
COPY . .

# Start the Bot
CMD [ "node", "index.js" ]

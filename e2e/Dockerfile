FROM mcr.microsoft.com/playwright:v1.58.2

WORKDIR /e2e

# Copy package files and install dependencies
COPY e2e/package*.json ./
RUN npm ci --frozen-lockfile

# Copy test files
COPY e2e/ .

# Ensure BASE_URL is provided as environment variable
ENV BASE_URL=""
# Enable certificate errors to be ignored for testing environments
ENV IGNORE_HTTPS_ERRORS=true
ENV NODE_TLS_REJECT_UNAUTHORIZED=0

# Create directory for test results
RUN mkdir -p test-results

# Run tests with BASE_URL environment variable
ENTRYPOINT ["npm", "run"]
CMD ["test:smoke"]

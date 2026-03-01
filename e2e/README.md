# Game Manager E2E Smoke Tests

Playwright-based smoke test suite for validating Game Manager deployments. Tests cover critical user journeys (create game → join game → start game) and verify successful application deployment.

## Quick Start

### Prerequisites
- Node.js 20+ 
- npm or yarn
- Running Game Manager instance (localhost:5000 for local development)

### Local Setup

```bash
# Install dependencies
npm ci

# Run tests against localhost (default)
npm run test:smoke

# Run tests with visible browser
npm run test:smoke:headed

# Open Playwright UI for debugging
npm run test:smoke:ui

# View HTML test report
npm run test:smoke:report
```

## Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `BASE_URL` | Yes | - | Base URL of the application (e.g., `http://localhost:5000`) |
| `TEST_TIMEOUT` | No | `30000` | Individual test timeout in milliseconds |
| `TEST_RETRIES` | No | `1` | Number of retries for flaky tests |
| `HEADLESS` | No | `true` | Run in headless mode (set to `false` for visible browser) |
| `LOG_LEVEL` | No | `info` | Logging level (trace, debug, info, warn, error) |
| `IGNORE_HTTPS_ERRORS` | No | `false` | Ignore HTTPS certificate verification errors (set to `true` for self-signed certs) |

### Examples

```bash
# Test against development environment
BASE_URL=http://localhost:5000 npm run test:smoke

# Test against staging with retries
BASE_URL=https://staging.example.com TEST_RETRIES=2 npm run test:smoke

# Test with self-signed certificates (ignore HTTPS errors)
BASE_URL=https://staging.example.com IGNORE_HTTPS_ERRORS=true npm run test:smoke

# Test with debugging
BASE_URL=http://localhost:5000 npm run test:smoke:headed
```

## Docker Build & Run

### Build Image

```bash
docker build -t game-manager-smoke:latest -f e2e/Dockerfile .
```

### Run Tests Locally

```bash
# Test against local app (using host's localhost)
docker run \
  -e BASE_URL=http://host.docker.internal:5000 \
  -v $(pwd)/e2e/test-results:/e2e/test-results \
  game-manager-smoke:latest

# Exit code: 0 = success, 1 = test failure
echo $?
```

### Mount Test Results Volume

```bash
docker run \
  -e BASE_URL=http://host.docker.internal:5000 \
  -v test-results:/e2e/test-results \
  game-manager-smoke:latest

# Test results will be available in test-results/ directory
ls -la test-results/html-report/
```

## Kubernetes Integration

### Prerequisites
- Kubernetes cluster (v1.20+)
- Application deployed and accessible via internal DNS or LoadBalancer

### Basic Job Manifest

```yaml
apiVersion: batch/v1
kind: Job
metadata:
  name: game-manager-smoke-tests
  namespace: default
spec:
  template:
    spec:
      containers:
      - name: smoke-tests
        image: game-manager-smoke:latest
        imagePullPolicy: IfNotPresent
        env:
        - name: BASE_URL
          value: "http://game-manager:5000"
        - name: TEST_TIMEOUT
          value: "30000"
        - name: TEST_RETRIES
          value: "2"
        volumeMounts:
        - name: test-results
          mountPath: /e2e/test-results
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
      volumes:
      - name: test-results
        emptyDir: {}
      restartPolicy: Never
  backoffLimit: 3
  ttlSecondsAfterFinished: 3600
```

### Post-Deployment Validation Job

Run tests immediately after deploying the application to validate a successful deployment:

```yaml
apiVersion: batch/v1
kind: Job
metadata:
  name: game-manager-post-deploy-tests
  namespace: default
  labels:
    app: game-manager
    type: smoke-test
spec:
  template:
    metadata:
      labels:
        app: game-manager
    spec:
      serviceAccountName: default
      containers:
      - name: smoke-tests
        image: ghcr.io/your-org/game-manager-smoke:latest
        imagePullPolicy: Always
        env:
        - name: BASE_URL
          value: "http://$(GAME_MANAGER_SERVICE_HOST):$(GAME_MANAGER_SERVICE_PORT)"
        - name: TEST_TIMEOUT
          value: "45000"  # Longer timeout for post-deploy
        - name: TEST_RETRIES
          value: "3"
        - name: LOG_LEVEL
          value: "debug"
        - name: IGNORE_HTTPS_ERRORS
          value: "false"  # Set to "true" for self-signed certificates
        volumeMounts:
        - name: test-results
          mountPath: /e2e/test-results
        livenessProbe:
          exec:
            command: ["ls", "/e2e/test-results"]
          initialDelaySeconds: 5
          periodSeconds: 10
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
      volumes:
      - name: test-results
        emptyDir: {}
      restartPolicy: Never
  backoffLimit: 1
  ttlSecondsAfterFinished: 3600
```

### Run Job

```bash
# Create and run job
kubectl apply -f smoke-tests-job.yaml

# Watch job progress
kubectl get jobs -w
kubectl describe job game-manager-smoke-tests

# View pod logs
kubectl logs -f job/game-manager-smoke-tests

# Check test results (if volume persisted)
kubectl exec simple-job-xxxxx -- cat /e2e/test-results/junit.xml
```

### GitHub Actions Integration

Add to your CI/CD workflow after deployment:

```yaml
- name: Run Smoke Tests
  if: success()
  run: |
    docker run \
      -e BASE_URL=${{ env.APP_URL }} \
      -e TEST_RETRIES=2 \
      -e TEST_TIMEOUT=45000 \
      -v ${{ github.workspace }}/test-results:/e2e/test-results \
      game-manager-smoke:${{ github.sha }}
      
- name: Upload Test Results
  if: always()
  uses: actions/upload-artifact@v3
  with:
    name: smoke-test-results
    path: test-results/
```

## Test Coverage

### Test Specs

- **01-create-game.spec.ts**: Game creation, entry code generation, unique IDs
- **02-join-game.spec.ts**: Player join, authentication, JWT tokens, multi-player support
- **03-start-game.spec.ts**: Game state transitions, player persistence, lifecycle validation
- **04-app-load.spec.ts**: Application loading, asset loading, responsiveness, error handling

### Critical Flows Validated

1. ✅ Create game with trackers
2. ✅ Receive unique entry code
3. ✅ Join game with entry code
4. ✅ Receive JWT authentication token
5. ✅ Authenticate subsequent API requests
6. ✅ Support multiple players in same game
7. ✅ Transition game to started state
8. ✅ Maintain player list through state transitions
9. ✅ Application homepage loads
10. ✅ Assets and JavaScript load correctly

## Troubleshooting

### Tests timeout connecting to application

**Issue**: `ERR_CONNECTION_REFUSED` or timeout errors

**Solutions**:
- Verify `BASE_URL` is correct and reachable from test environment
- Check application logs: `docker logs game-manager` or `kubectl logs deployment/game-manager`
- For Docker: Use `http://host.docker.internal:5000` instead of `http://localhost:5000`
- For Kubernetes: Use DNS name of service (e.g., `http://game-manager:5000`)
- Add longer timeout: `TEST_TIMEOUT=60000`

### Browser launch failures

**Issue**: `Error: Browser is not installed`

**Solution**:
Build image includes Playwright browser installation. If running outside container:
```bash
npx playwright install chromium
```

### Test results not captured

**Issue**: Test results volume not mounted properly

**Solution**:
```bash
# Ensure volume path exists locally
mkdir -p test-results

# Run with explicit volume mount
docker run -v "$(pwd)/e2e/test-results:/e2e/test-results" ...
```

### Authentication failures

**Issue**: 401 Unauthorized responses

**Solutions**:
- Verify API is accepting requests (test 04-app-load passes)
- Check that game creation returns valid entry codes
- Examine token payload: See `02-join-game.spec.ts`
- Review application authentication logs

## Development

### Add New Tests

```bash
# Create new test file in tests/specs/
touch e2e/tests/specs/05-your-feature.spec.ts

# Write test using existing helpers
import { test, expect } from '@playwright/test';
import { config, validateConfig } from '../src/config/env.config';

test.beforeAll(() => validateConfig());

test('your test', async ({ page }) => {
  // Your test code
});
```

### Add New Helpers

Place helper utilities in `src/helpers/` and import them:

```typescript
import { yourHelper } from '../src/helpers/your-helper';

test('test using helper', async () => {
  const result = await yourHelper(config.baseUrl);
});
```

## Best Practices

- ✅ Use `generateGameName()` for unique test data
- ✅ Keep tests independent (no test dependencies)
- ✅ Use existing helpers for common operations
- ✅ Log meaningful assertions with `console.log()`
- ✅ Fail fast with clear error messages
- ✅ Configure appropriate timeouts for your environment
- ✅ Validate both API responses and UI state
- ⚠️ Don't hardcode URLs, use `config.baseUrl`
- ⚠️ Don't create test data cleanup (reuse existing DB state)
- ⚠️ Don't test internal implementation details

## Reporting

### HTML Report

```bash
npm run test:smoke:report
```

Opens interactive HTML report in browser showing:
- Test results and execution time
- Screenshots and videos of failures
- Trace information for debugging

### Reporting

Reports generated to:
- `test-results/html-report/index.html` - Interactive HTML report
- `test-results/artifacts/` - Screenshots, videos, and traces for failed tests
- `test-results/results.json` - Machine-readable results
- `test-results/junit.xml` - JUnit format for CI tools

### Metrics

Track important metrics across deployments:
- Test execution time
- Pass/fail rate
- Timeout occurrences
- Platform/environment performance

## Maintenance

- Review and update tests when API contracts change
- Monitor timeout values for performance regressions
- Keep Playwright dependencies updated: `npm update @playwright/test`
- Archive test reports for trend analysis

## License

See LICENSE in project root.

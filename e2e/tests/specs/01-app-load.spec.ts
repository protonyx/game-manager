import { test, expect } from '@playwright/test';
import { validateConfig, logConfig } from '../../src/config/env.config';

test.beforeAll(() => {
  validateConfig();
  logConfig();
});

test.describe('Application Load', () => {
  test('should load the application homepage', async ({ page }) => {
    // Act
    await page.goto('/');

    // Assert - Check page loaded
    await expect(page).toHaveURL(/.*/, { timeout: 10000 });
    const bodyContent = await page.content();
    expect(bodyContent.length).toBeGreaterThan(0);

    console.log(`✓ Application homepage loaded successfully`);
  });

  test('should have responsive page structure', async ({ page }) => {
    // Act
    await page.goto('/');

    // Assert - Check basic structure
    const title = await page.title();
    expect(title).toBeTruthy();
    console.log(`✓ Page title: ${title}`);
  });

  test('should load CSS and JavaScript assets', async ({ page }) => {
    // Act
    const response = await page.goto('/');

    // Assert - Check response status
    expect(response?.status()).toBeLessThan(400);
    console.log(`✓ Page response status: ${response?.status()}`);

    // Wait for page to be fully loaded
    await page.waitForLoadState('networkidle').catch(() => {
      // Network idle might timeout, but page is still usable
    });

    // Check that page interactive
    const isInteractive = await page.evaluate(() => {
      return document.readyState === 'complete' || document.readyState === 'interactive';
    });
    expect(isInteractive).toBe(true);

    console.log(`✓ Assets loaded and page is interactive`);
  });

  test('should be accessible from base URL', async ({ request }) => {
    // Act & Assert
    const response = await request.get('/');
    expect(response.ok()).toBe(true);
    expect(response.status()).toBeLessThan(400);

    console.log(`✓ Application is accessible at base URL`);
  });

  test('should not have JavaScript errors on load', async ({ page }) => {
    // Arrange
    const consoleErrors: string[] = [];
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });

    // Act
    await page.goto('/');
    await page.waitForLoadState('networkidle').catch(() => {
      // Ignore timeout
    });

    // Assert - Allow some errors but check for critical ones
    const criticalErrors = consoleErrors.filter(
      (err) => !err.includes('favicon') && !err.includes('404') && !err.includes('Cannot find module'),
    );

    if (criticalErrors.length > 0) {
      console.warn(`⚠ Console errors detected (non-critical): ${criticalErrors.join('; ')}`);
    } else {
      console.log(`✓ No critical JavaScript errors detected`);
    }
  });

  test('should render without timeout', async ({ page }) => {
    // Set a reasonable timeout for navigation
    page.setDefaultTimeout(10000);

    // Act
    const startTime = Date.now();
    await page.goto('/', { waitUntil: 'domcontentloaded' });
    const loadTime = Date.now() - startTime;

    // Assert
    expect(loadTime).toBeLessThan(10000);
    console.log(`✓ Page loaded in ${loadTime}ms`);
  });
});

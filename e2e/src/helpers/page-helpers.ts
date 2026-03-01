/**
 * Page interaction helpers
 * Provides utilities for common UI interactions and assertions
 */

import { Page, expect } from '@playwright/test';

/**
 * Wait for and find element by test ID
 * @param page Playwright page object
 * @param testId Test ID attribute value
 * @param timeout Optional timeout in milliseconds
 */
export async function getByTestId(page: Page, testId: string, timeout = 5000) {
  return page.locator(`[data-testid="${testId}"]`).first().waitFor({ timeout });
}

/**
 * Click a button by its text content
 * @param page Playwright page object
 * @param buttonText Text content of the button
 * @param timeout Optional timeout in milliseconds
 */
export async function clickButtonByText(page: Page, buttonText: string, timeout = 5000) {
  await page.locator(`button:has-text("${buttonText}")`).first().click({ timeout });
}

/**
 * Fill input field by label text
 * @param page Playwright page object
 * @param labelText Label text associated with input
 * @param value Value to fill
 * @param timeout Optional timeout in milliseconds
 */
export async function fillInputByLabel(page: Page, labelText: string, value: string, timeout = 5000) {
  const label = page.locator(`label:has-text("${labelText}")`);
  const input = label.locator('.. input').first();
  await input.fill(value, { timeout });
}

/**
 * Wait for page to load and be interactive
 * @param page Playwright page object
 * @param timeout Optional timeout in milliseconds
 */
export async function waitForPageLoad(page: Page, timeout = 10000) {
  await page.waitForLoadState('networkidle', { timeout }).catch(() => {
    // Network idle may timeout, but page might still be usable
  });
}

/**
 * Assert that element is visible on page
 * @param page Playwright page object
 * @param testId Test ID of element to check
 */
export async function assertElementVisible(page: Page, testId: string) {
  const element = page.locator(`[data-testid="${testId}"]`).first();
  await expect(element).toBeVisible();
}

/**
 * Assert text content of element
 * @param page Playwright page object
 * @param testId Test ID of element
 * @param expectedText Expected text content
 */
export async function assertTextContent(page: Page, testId: string, expectedText: string) {
  const element = page.locator(`[data-testid="${testId}"]`).first();
  await expect(element).toContainText(expectedText);
}

/**
 * Wait for specific text to appear on page
 * @param page Playwright page object
 * @param text Text to wait for
 * @param timeout Optional timeout in milliseconds
 */
export async function waitForText(page: Page, text: string, timeout = 5000) {
  await page.locator(`text=${text}`).waitFor({ timeout });
}

/**
 * Take screenshot for debugging
 * @param page Playwright page object
 * @param name Name of the screenshot file
 */
export async function takeScreenshot(page: Page, name: string) {
  await page.screenshot({ path: `test-results/screenshots/${name}.png` });
}

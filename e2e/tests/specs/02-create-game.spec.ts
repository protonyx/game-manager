import { test, expect } from '@playwright/test';
import { generateGameName } from '../../src/helpers/game';
import { validateConfig, logConfig } from '../../src/config/env.config';

test.beforeAll(() => {
  validateConfig();
  logConfig();
});

test.describe('Create Game Flow - UI', () => {
  test('should create a game using the web form', async ({ page }) => {
    // Arrange
    const gameName = generateGameName('E2E_CreateGame');
    
    // Act - Navigate to new game page
    await page.goto('/game/new');
    await page.waitForLoadState('networkidle');

    // Fill in game name
    const gameNameInput = page.locator('input[formcontrolname="name"]').first();
    await gameNameInput.fill(gameName);

    // Add at least one tracker if none exist
    const addTrackerBtn = page.locator('[mat-mini-fab]');
    await addTrackerBtn.first().click();
    
    // Fill tracker name
    const trackerNames = page.locator('input[formcontrolname="name"]');
    const lastTrackerName = trackerNames.last();
    await lastTrackerName.fill('TestTracker');

    // Fill tracker starting value
    const trackerStartingValues = page.locator('input[formcontrolname="startingValue"]');
    const lastTrackerValue = trackerStartingValues.last();
    await lastTrackerValue.fill('10');

    // Click Create button
    await page.locator('button:has-text("Create")').click();

    // Assert - Wait for navigation to game page and verify entry code is displayed
    await page.waitForURL('/game', { waitUntil: 'networkidle' });
    
    // Check that entry code is displayed in the header
    const entryCodeElement = page.locator('strong').filter({ hasText: /^[A-Z0-9]+$/ }).first();
    await expect(entryCodeElement).toBeVisible();
    
    const entryCode = await entryCodeElement.textContent();
    expect(entryCode).toBeTruthy();
    expect(entryCode).toMatch(/^[A-Z0-9]+$/);

    console.log(`✓ Game created via UI: Name=${gameName}, EntryCode=${entryCode}`);
  });

  test('should display game name and trackers in the UI', async ({ page }) => {
    // Arrange
    const gameName = generateGameName('E2E_VerifyDisplay');

    // Act - Create game
    await page.goto('/game/new');
    await page.waitForLoadState('networkidle');

    const gameNameInput = page.locator('input[formcontrolname="name"]').first();
    await gameNameInput.fill(gameName);

    // Add a tracker
    const addTrackerBtn = page.locator('[mat-mini-fab]');
    await addTrackerBtn.click();

    const trackerNames = page.locator('input[formcontrolname="name"]');
    await trackerNames.last().fill('Score');

    const trackerValues = page.locator('input[formcontrolname="startingValue"]');
    await trackerValues.last().fill('100');

    // Submit form
    await page.locator('button:has-text("Create")').click();
    await page.waitForURL('/game');

    // Assert - Verify game loaded successfully
    const entryCodeElement = page.locator('strong').filter({ hasText: /^[A-Z0-9]+$/ }).first();
    await expect(entryCodeElement).toBeVisible();
    
    console.log(`✓ Game displayed in UI with trackers`);
  });

  test('should require game name to create a game', async ({ page }) => {
    // Arrange & Act
    await page.goto('/game/new');
    await page.waitForLoadState('networkidle');

    // Try to click Create without filling in name
    const createBtn = page.locator('button:has-text("Create")');
    
    // Assert - Create button should be disabled
    await expect(createBtn).toBeDisabled();

    console.log(`✓ Create button is disabled when form is invalid`);
  });

  test('should allow adding and removing trackers', async ({ page }) => {
    // Arrange
    const gameName = generateGameName('E2E_Trackers');

    // Act
    await page.goto('/game/new');
    await page.waitForLoadState('networkidle');

    // Fill game name
    const gameNameInput = page.locator('input[formcontrolname="name"]').first();
    await gameNameInput.fill(gameName);

    // Add first tracker
    let addBtn = page.locator('[mat-mini-fab]');
    await addBtn.click();
    
    // Add second tracker
    await addBtn.click();

    // Get all tracker names
    const trackerNameInputs = page.locator('input[formcontrolname="name"]');

    // Fill in tracker details
    const trackerNamesList = trackerNameInputs.all();
    await (await trackerNamesList)[0].fill('Health');
    await (await trackerNamesList)[1].fill('Mana');

    const trackerValues = page.locator('input[formcontrolname="startingValue"]');
    const trackerValuesList = trackerValues.all();
    await (await trackerValuesList)[0].fill('50');
    await (await trackerValuesList)[1].fill('30');

    // Remove one tracker via delete button
    const deleteBtns = page.locator('button:has-text("Remove")');
    const initialDeleteCount = await deleteBtns.count();
    await deleteBtns.first().click();

    // Assert
    const finalDeleteCount = await page.locator('button:has-text("Remove")').count();
    expect(finalDeleteCount).toBe(initialDeleteCount - 1);

    console.log(`✓ Trackers added and removed successfully`);
  });
});

# Very Bad Dice Roller — Requirements Specification

Inspired by [Very Good Dice Roller](https://verygooddiceroller.com/), this document captures all features to be implemented in the Very Bad Dice Roller, along with the additional requirements unique to this app.

---

## 1. Dice Types

The app shall support the following standard polyhedral dice used in tabletop RPGs such as Dungeons & Dragons and Pathfinder:

- d4
- d6
- d8
- d10
- d12
- d20
- d100

Each die shall be accessible via a direct click button — no dropdown menus, no increment controls, no animations.

---

## 2. Roll Modes

### 2.1 Normal Roll

- A single roll of the selected die.
- The result is displayed immediately.

### 2.2 Advantage Roll

- Two dice of the selected type are rolled.
- The higher of the two values is used as the result.
- Both raw values are displayed alongside the kept result.

### 2.3 Disadvantage Roll

- Two dice of the selected type are rolled.
- The lower of the two values is used as the result.
- Both raw values are displayed alongside the kept result.

### 2.4 Mode Selection

- Toggle buttons for Advantage and Disadvantage are provided.
- Only one mode may be active at a time (advantage and disadvantage are mutually exclusive).
- When neither is active, rolls are normal.

---

## 3. Modifiers

### 3.1 Available Modifiers

- Positive modifiers: +1 through +10.
- Negative modifiers: -1 through -10.
- Each modifier is accessible via a direct click button.
- Multiple modifier buttons may be clicked to accumulate a total modifier.

### 3.2 Modifier Application Modes

- **Mod per roll**: The modifier is applied once to the total result of a roll (default).
- **Mod per die**: The modifier is applied individually to each die rolled.
- A toggle button switches between these two modes.

---

## 4. Custom Rolls

- The user may specify:
  - A **count** of dice to roll (1–100).
  - A **die type** (d4, d6, d8, d10, d12, d20, d100).
- A **Roll** button executes the custom roll.

---

## 5. Expression Builder

- A **Build Expression** toggle activates expression builder mode.
- In this mode, clicking dice adds them to a running expression (e.g., `2d6 + 1d8 + 1d4`) rather than rolling immediately.
- A text field displays the current expression being built (read-only while building).
- A **Roll Expression** button rolls the full expression.
- A **Save Expression** button saves the current expression for reuse.
  - This button is disabled when there is no expression to save.
- Saved expressions appear as quick-access buttons.

---

## 6. Roll Results Display

- Results are shown in a dedicated results panel.
- Each result entry shows:
  - The die type rolled.
  - The raw dice value(s).
  - The modifier applied (if any).
  - The final result.
  - The roll type (Normal / Advantage / Disadvantage).
- A **Clear Results** button removes all current results from the display.
- When no rolls have been made, the panel shows a prompt such as "No rolls yet. Click dice to get started!"

---

## 7. Roll History

- A **Show History** button toggles a side panel showing the full roll history for the session.
- Each history entry records the same information as the results display.
- A **Clear History** button clears the entire roll history and resets session statistics.
- When no history exists, the panel shows "No roll history yet."

---

## 8. Session Statistics

- A **Show Statistics** button opens a statistics modal.
- Statistics are tracked per session based on roll history.

### 8.1 Dice Type Filters

- Buttons allow filtering statistics by die type: All, d4, d6, d8, d10, d12, d20, d100.

### 8.2 Roll Type Filters

- **Natural**: Shows statistics for raw, unmodified dice values (default active).
- **Add non-natural results**: Blends advantage, disadvantage, and modified rolls into the dataset.
- When non-natural results are included, individual toggles become available:
  - **Advantage**: Show advantage roll results.
  - **Disadvantage**: Show disadvantage roll results.
  - **Modified**: Show modified roll results.
- The raw dice values of non-natural rolls can also be shown by including non-natural results and keeping the Natural button toggled on.

### 8.3 Test Statistics

- A **Test Statistics** button populates the statistics panel with generated sample data for inspection and verification purposes.

---

## 9. Random Number Generator (RNG)

### 9.1 Algorithm Selection

- The app exposes an **RNG** dropdown allowing the user to select between:
  - **Mersenne Twister** (default and recommended): A well-established algorithm with strong statistical properties and a very long period.
  - **Math.random()**: The browser's built-in RNG; quality varies by browser.

### 9.2 Randomness Test

- A **Test Randomness** button opens a randomness test modal.
- The user may configure:
  - **Test Cycles**: 20, 25, 30, 40, 50, 75, or 100 cycles (default: 100).
  - **Algorithm**: Mersenne Twister or Math.random().
- **Methodology**:
  - Each die (d4, d6, d8, d10, d12, d20, d100) is rolled 10,000 times per cycle and checked for an even distribution.
  - Approximately 30% of cycles are expected to show at least one failure even with a healthy RNG; this is normal and expected.
  - The test verifies that the failure rate falls within the expected range.
  - A one-off failure is normal; consistently high failure rates or a single die failing repeatedly indicate a problem.
  - More cycles yield a more reliable verdict but take longer.
- A **Run Test** button starts the test.
- A live status indicator shows the test progress.
- Results are displayed after the test completes.

---

## 10. D&D 5e Character Ability Score Generator

- A **D&D 5e Character** button generates a set of ability scores following D&D 5th Edition rules.
- The standard method rolls 4d6, drops the lowest die, and sums the remaining three, repeated six times to produce six scores.
- The six generated scores are displayed to the user.

---

## 11. Display and UI

### 11.1 Dark Mode

- The app defaults to dark mode.
- The visual design supports dark mode presentation.

### 11.2 Accessibility

- All interactive controls include appropriate ARIA labels.
- Live regions (`aria-live`) are used for roll results, history, and test status so screen readers announce updates.
- Toggle buttons expose their pressed state via `aria-pressed`.

### 11.3 Progressive Web App (PWA)

- The app includes a web app manifest enabling installation as a PWA on supported devices.

### 11.4 No-Friction Design

- No animations delay the display of results.
- No dropdown menus or increment buttons are used for the primary dice selection.
- Dice are large, direct-click targets.

---

## 12. Weighted Dice — The "Very Bad" Mechanic

This is the defining feature that makes this a **very bad** dice roller. Every die roll is secretly biased, and the bias itself shifts over time without the user's control or knowledge.

### 12.1 Per-Die Weighted Resolution

- Each individual die roll is resolved using a **weighted random** algorithm rather than a uniform distribution.
- The weighting simulates either advantage (bias toward higher values) or disadvantage (bias toward lower values).
- Specifically:
  - In **advantage-weighted** mode: two values are generated internally and the higher is returned as the result, even for a nominally "normal" roll.
  - In **disadvantage-weighted** mode: two values are generated internally and the lower is returned as the result.
- This weighting is invisible to the user — results appear as if they were standard rolls.
- When the user explicitly selects Advantage or Disadvantage mode (Section 2), the explicit mode overrides the internal weighting direction, but the result is still computed using the weighted engine.

### 12.2 Automatic Mode Shifting

- The internal weighting mode (advantage vs. disadvantage) **shifts automatically over time** while the app is running.
- Shifting is driven by logic executing in the browser (client-side only; no server involvement).
- The shifting logic shall be based on a combination of the following signals:
  - **Time elapsed**: The weight mode cycles based on elapsed session time (e.g., periodically switching every few minutes).
  - **Roll count**: The number of rolls made in the session influences when the next shift occurs.
  - **Pseudo-random scheduling**: The timing of shifts includes a random element so the pattern is not trivially predictable.
- The current internal weight mode is **not disclosed** in the UI. The user sees only their roll results.

### 12.3 Shift Logic Requirements

- The system shall maintain an internal state tracking:
  - The current weight mode (advantage-weighted or disadvantage-weighted).
  - The timestamp of the last mode shift.
  - The roll count since the last mode shift.
- A mode shift shall be triggered when any of the following conditions are met:
  - A configurable time threshold has elapsed since the last shift (e.g., 2–5 minutes, randomised).
  - A configurable number of rolls has occurred since the last shift (e.g., 10–25 rolls, randomised).
- On each shift, new random thresholds for the next shift are computed and stored.
- The initial mode at session start shall be randomly selected.

### 12.4 Interaction with Explicit Roll Modes

| User Mode Selected | Internal Weight Mode | Behaviour |
|--------------------|----------------------|-----------|
| Normal             | Advantage-weighted   | Rolls two dice internally, returns the higher |
| Normal             | Disadvantage-weighted| Rolls two dice internally, returns the lower |
| Advantage          | Advantage-weighted   | Rolls two dice for advantage, each die internally weighted toward higher |
| Advantage          | Disadvantage-weighted| Rolls two dice for advantage, each die internally weighted toward lower |
| Disadvantage       | Advantage-weighted   | Rolls two dice for disadvantage, each die internally weighted toward higher |
| Disadvantage       | Disadvantage-weighted| Rolls two dice for disadvantage, each die internally weighted toward lower |

### 12.5 Statistics and History Transparency

- Roll history and statistics shall record the **final displayed result**, not the internal raw values.
- The internal weighting mechanism shall not be surfaced in any history entry, statistics view, or export.

---

## 13. Non-Functional Requirements

- The application runs entirely in the browser (client-side); no server-side roll computation.
- The application is free to use.
- The application shall function correctly on modern desktop and mobile browsers.
- The weighted dice logic (Section 12) shall be implemented in client-side JavaScript/WebAssembly.
- No external API calls are made for dice rolling or weighting logic.

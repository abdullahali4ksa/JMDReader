# 🚀 Microsoft Partner Center & Windows Store Publishing Guide

> **Official Publishing Guide for birotech**
>
> Target App: **MDReader** (Markdown Document Reader & Editor for Windows)
>
> Publisher: **birotech** (`Abdullah Ali`)

---

## Step 1: Complete Developer Account Registration (Current Browser Tab)

You are currently on the Partner Center **Address Information** verification screen:
1. Review the verified address:
   * **Country/Region:** `Saudi Arabia`
   * **State/Province:** `Al Jawf`
   * **City:** `Sakakah`
   * **Street:** `Omar bin Alkhatab`
   * **ZIP Code:** `72341`
2. Click the blue **Continue** button.
3. In the next **Publisher Profile** screen:
   * **Publisher Display Name:** Enter `birotech`
   * **Contact Name:** `Abdullah Ali`
   * **Email Address:** `Abdullah444ali@outlook.com`
   * **Phone Number:** `+966554821381`
4. In the **Payment & Agreement** screen:
   * Microsoft requires a one-time developer registration fee ($19 USD for Individual account).
   * Enter your preferred payment method (Credit Card / Mada / PayPal) and complete registration.

---

## Step 2: Reserve the App Name

1. In the Partner Center Dashboard, navigate to **Apps and games** -> **Overview**.
2. Click **Create a new app** (or **New product**).
3. Under **App name**, type `MDReader`.
4. Click **Check availability** -> then click **Reserve product name**.

---

## Step 3: Configure Store Listings & Submission

On the submission checklist, configure the following:

### 1. Pricing and Availability
* **Base price:** Free
* **Markets:** All markets (Worldwide)
* **Discoverability:** Publicly available in the Microsoft Store

### 2. Properties
* **Category:** `Productivity` -> `Document management`
* **Privacy Policy URL:** `https://github.com/ABDULLAHALI4KSA/MDReader#privacy-policy`
* **Support Contact Email:** `Abdullah444ali@outlook.com`
* **Website:** `https://github.com/ABDULLAHALI4KSA/MDReader`

### 3. Age Ratings
* Complete the IARC questionnaire:
  * Contains violence? No
  * Contains bad language? No
  * Collects user personal data? No
  * Online interactions? No
* Result: **All Ages / PEGI 3 / ESRB Everyone**.

### 4. Store Listings (English - United States & Arabic)
* **Title:** `MDReader — Modern Markdown Document Reader`
* **Short Description:** `A high-performance, distraction-free Markdown document reader and editor with GFM tables, alerts, math formulas, and teleprompter auto-scroll.`
* **Full Description:**
  ```markdown
  MDReader is a modern, ultra-fast Markdown reader and editor built natively for Windows 10 & 11.

  Key Features:
  - Distraction-Free Reading Mode: Clean full-width reading experience with high-contrast typography.
  - Teleprompter Smooth Auto-Scroll: Continuous fluid scrolling with customizable speeds (0.25x to 2x or custom).
  - High-Fidelity Rendering: GitHub-Flavored Markdown (GFM) tables, GitHub alerts, KaTeX math formulas, and Mermaid diagrams.
  - Multi-Language Support: Instant bilingual and multilingual UI for 10 world languages.
  - Standalone Offline HTML Export: Share interactive self-contained documents that work in any browser without internet.
  - Interactive Table of Contents & Headings Observer.
  - Notes & Annotations Drawer for selected text quotes.
  ```
* **Search Terms / Keywords:** `markdown, md reader, markdown viewer, documentation, editor, teleprompter, birotech`

---

## Step 4: Packages Upload

1. Under **Packages**, upload either:
   * The **Inno Setup Installer** executable: `dist/installer/MDReader-Setup-x64.exe` (via Win32 app submission).
   * OR the packaged `.msixupload` bundle built with certificate `packaging/certs/birotech_Dev.pfx`.
2. Click **Review and submit**.
3. Microsoft Store Certification typically approves within 24 to 72 hours.

# UI Design — MDReader

## Design Principles

1. **Simplicity** — Minimal chrome, focus on content
2. **Speed** — Fast rendering, no unnecessary animations
3. **Native behavior** — Follow Windows 11 conventions
4. **Readability** — Typography optimized for reading
5. **Accessibility** — Keyboard navigation, screen reader support
6. **Arabic support** — Full RTL layout, Arabic font rendering
7. **Light/Dark theme** — Follows system, manual toggle
8. **High DPI** — Sharp on all scaling factors
9. **Keyboard support** — All actions keyboard-accessible

## Layout (MVP)

```
┌─────────────────────────────────────────┐
│  Title Bar (custom, extended content)   │
│  ┌─────────────────────────────────────┐ │
│  │  Menu: File | View | Settings       │ │
│  ├─────────────────────────────────────┤ │
│  │  Reader Pane                         │ │
│  │  ┌───────────────────────────────┐  │ │
│  │  │ Markdown Content                │  │ │
│  │  │                               │  │ │
│  │  └───────────────────────────────┘  │ │
│  └─────────────────────────────────────┘ │
└─────────────────────────────────────────┘
```

## Color Palette

| Token | Light | Dark |
|---|---|---|
| Background | #FFFFFF | #1E1E1E |
| Text | #1F1F1F | #E0E0E0 |
| Accent | #0078D4 | #00B4F0 |
| Border | #D1D5DB | #383838 |

Colors are defined using Windows 11 system resource keys where possible.

## Typography

| Element | Font | Size |
|---|---|---|
| Body | Segoe UI Variable | 16sp |
| Heading 1 | Segoe UI Variable | 24sp |
| Heading 2 | Segoe UI Variable | 20sp |
| Heading 3 | Segoe UI Variable | 18sp |
| Code block | Cascadia Code | 14sp |

## Arabic / RTL Support

- `FlowDirection` set to `RightToLeft` when Arabic content is detected
- Arabic font fallback chain: Segoe UI Variable → Cairo → Amiri
- Mixed RTL/LTR content handled per-paragraph via Unicode bidirectional algorithm
- Layout mirrors automatically (navigation on right for RTL)

## Components

### Shell (Window)
- Extended content into title bar
- Custom title bar with app icon
- Menu bar: File | View | Settings

### Reader
- Scrollable content area
- Syntax-highlighted code blocks (Phase 2)
- Inline image rendering (safe subset)
- Link hover tooltips

### FileOpen
- Windows-style file dialog
- Filter: Markdown files (*.md, *.markdown)
- Recent files list
- Drag-and-drop support

### Settings
- Theme selector (Light / Dark / System)
- Language selector (English / Arabic)
- Startup options (restore last session)

### Theme
- System theme detection
- Theme persistence via app settings

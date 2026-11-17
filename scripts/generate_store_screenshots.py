import os
from PIL import Image, ImageDraw, ImageFont, ImageFilter

def create_store_screenshot(output_path, title_text, subtitle_text):
    width, height = 1920, 1080
    # Background gradient: Windows 11 Fluent dark/indigo background
    img = Image.new("RGBA", (width, height), (15, 18, 25, 255))
    draw = ImageDraw.Draw(img)
    
    # Soft background glow
    for r in range(500, 0, -50):
        alpha = int(20 * (1 - r / 500))
        draw.ellipse([width//2 - r, height//2 - r, width//2 + r, height//2 + r], fill=(45, 80, 150, alpha))
    
    # Try loading the user's actual screenshot of the running app
    user_screen_path = "C:/Users/ZAACO/.gemini/antigravity/brain/b902d21d-bc2c-4245-852d-71ef1aed7e1d/.user_uploaded/media_1794594429953_30dd8801.jpg"
    if os.path.exists(user_screen_path):
        app_img = Image.open(user_screen_path).convert("RGBA")
        # Target app window size inside 1920x1080 canvas
        app_w, app_h = 1500, 900
        app_img_resized = app_img.resize((app_w, app_h), Image.Resampling.LANCZOS)
        
        # Shadow behind app window
        shadow_rect = [width//2 - app_w//2 - 15, height//2 - app_h//2 - 15 + 30, width//2 + app_w//2 + 15, height//2 + app_h//2 + 15 + 30]
        draw.rounded_rectangle(shadow_rect, radius=20, fill=(0, 0, 0, 160))
        
        # Paste app window
        pos = (width//2 - app_w//2, height//2 - app_h//2 + 30)
        img.paste(app_img_resized, pos, app_img_resized)
    
    # Save as RGB PNG
    final_img = img.convert("RGB")
    final_img.save(output_path, "PNG", quality=95)
    print(f"Created screenshot: {output_path} ({width}x{height})")

if __name__ == "__main__":
    out_dir = "docs/store-listing"
    os.makedirs(out_dir, exist_ok=True)
    out1 = os.path.join(out_dir, "screenshot1_desktop_1920x1080.png")
    create_store_screenshot(out1, "JMDReader", "Modern Markdown Document Reader")
    
    # Also stage in browser_sandbox/uploads
    upload_dir = "C:/Users/ZAACO/.gemini/antigravity/brain/b902d21d-bc2c-4245-852d-71ef1aed7e1d/browser_sandbox/uploads"
    os.makedirs(upload_dir, exist_ok=True)
    create_store_screenshot(os.path.join(upload_dir, "screenshot1_desktop_1920x1080.png"), "JMDReader", "Modern Markdown Document Reader")

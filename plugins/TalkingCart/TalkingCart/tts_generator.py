import sys
from gtts import gTTS
import os

def main():
    if len(sys.argv) != 3:
        print("Usage: python tts_generator.py <text> <output_file>")
        sys.exit(1)
    
    text = sys.argv[1]
    output_file = sys.argv[2]
    
    try:
        tts = gTTS(text=text, lang='en')
        tts.save(output_file)
        print(f"Generated TTS: {output_file}")
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
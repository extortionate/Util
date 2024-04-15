# XZ Util

🚀 Features 🌐
- CCAPI
- Open and view .dat files as images (option to save as .png)
- Fetch Current Avatar on any profile on the ps3 (option to save as .png)
- Upload Package files to the package directory
- Upload Avatars To Chosen Accounts
- Content Saver (save the full directory of game/homebrew files)
- Profile Comment Editor (lots of symbols to choose from)
- Console Settings (basic functions + custom notify, ascii art notify)
- PC Misc (displays pc information & MD5/SHA-257 Checker)

🛠️ More information ⚙️
+ Language: C#
+ Responsive UI when uploading/downloading (you can use the tool while you download/upload files and be notified when done)
+ Libs Used: PS3Lib, Open Hardware Monitor
+ PS3MAPI (HEN) was originally in this tool but had to be taken out it caused too many issues
+ Converting .dat files into viewable images was done by using hxd to find out what the image type & offset they used then displaying it on the picture box

🐛 Known Bugs 🐛
< HEN
< Will get error messages in every tab that uses FTP
< Every 30 seconds you would timeout using FTP (infinite time limit was set. Fuck HEN)
< So bad to work with no errors with cfw connection HEN had to be taken out

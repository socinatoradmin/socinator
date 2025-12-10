# Socinator
Socinator is the ultimate social media marketing software. With a seamless, easy-to-use interface and powerful automation features, it lets you streamline your social media workflow.

# Socinator – Open Source Edition
### Automated Social Network Management Tool (WPF + Prism + .NET Framework 4.8)

## 📌 Overview

Socinator is a desktop-based Social Network Automation Tool built using:

- .NET Framework 4.8
- WPF (Windows Presentation Foundation)
- Prism Modular Framework
- Unity Dependency Injection
- SQLite Local Database
- Binary Storage (bin file) Processing Concepts

This open-source edition provides the full modular architecture for managing multiple platforms—each platform isolated in separate branches as individual libraries.

---

## 🏗️ Technology Stack

| Component       | Technology                             |
|----------------|-----------------------------------------|
| UI Framework   | WPF (.NET Framework 4.8)                |
| Architecture   | Prism MVVM + Region Navigation          |
| DI Container   | Unity                                   |
| Database       | SQLite Local DB                         |
| Data Handling  | BIN file processing + encrypted storage |
| Modularity     | Network modules separated into branches |

---

## 🧩 Branch Structure (Official)

From your GitHub repository, these are the actual branches:

```
dominatorhouse-social        → Main/Core WPF Application
facedominator-library        → Facebook Module
gramdominator-library        → Instagram Module
pindominator-library         → Pinterest Module
quoradominator-library       → Quora Module
reddit-dominator-library     → Reddit Module
tumbrldominator-library      → Tumblr Module
twtdominator-library         → Twitter/X Module
youtubedominator-library     → YouTube Module
```

Each branch contains the required module code, views, viewmodels, and handlers for its respective network.

---

## 📁 Folder & Cloning Structure (Very Important)

Socinator uses a modular architecture where each network must be cloned into a separate folder under a main directory named Socinator.

### 1️⃣ Create the root folder

```
mkdir Socinator
cd Socinator
```

### 2️⃣ Clone Main / Core App

```
git clone -b dominatorhouse-social https://github.com/socinatoradmin/socinator.git dominatorhouse-social
```

Folder created:

```
Socinator/dominatorhouse-social
```

### 3️⃣ Clone Each Network Module (Library Projects)

```
git clone -b facedominator-library https://github.com/socinatoradmin/socinator.git facedominator-library
git clone -b gramdominator-library https://github.com/socinatoradmin/socinator.git gramdominator-library
git clone -b pindominator-library https://github.com/socinatoradmin/socinator.git pindominator-library
git clone -b quoradominator-library https://github.com/socinatoradmin/socinator.git quoradominator-library
git clone -b reddit-dominator-library https://github.com/socinatoradmin/socinator.git reddit-dominator-library
git clone -b tumblrdominator-library https://github.com/socinatoradmin/socinator.git tumblrdominator-library
git clone -b twtdominator-library https://github.com/socinatoradmin/socinator.git twtdominator-library
git clone -b youtubedominator-library https://github.com/socinatoradmin/socinator.git youtubedominator-library
```

---

## 🗂️ Final Folder Structure (Correct Layout)

```
Socinator/
│
├── dominatorhouse-social/          → Main WPF Prism App
│
├── facedominator-library/          → Facebook module
├── gramdominator-library/          → Instagram module
├── pindominator-library/           → Pinterest module
├── quoradominator-library/         → Quora module
├── reddit-dominator-library/       → Reddit module
├── tumblrdominator-library/        → Tumblr module
├── twtdominator-library/           → Twitter/X module
└── youtubedominator-library/       → YouTube module
```

---

## 📦 How the Application Loads Modules

Prism + Unity dynamically loads each module using:

- IModule → Module Initialization
- IUnityContainer.RegisterType()
- RegionManager.RegisterViewWithRegion()

If a library folder is missing → Prism simply skips that module.

This provides a plugin-based architecture.

---

## 🗄️ Database & BIN File System

Socinator uses:

### ✔ SQLite Local Database

Stored inside:

```
/AppData/Local/Socinator1.0/
```

### ✔ BIN File Concept

Used for:

- Account storage
- Session tokens
- Cookies
- Temporary network data
- Encrypted user configurations

All handled by shared utility classes inside the core project.

---

## 🔨 Building the Solution

### Prerequisites

- Visual Studio 2022 / 2019
- .NET Framework Developer Pack 4.8
- SQLite NuGet Packages
- Prism + Unity Extensions installed

### Build Steps

1. Open "DominatorHouse.sln" inside dominatorhouse-social folder 
2. Add references:
   - Each network module project  
3. Build in Release mode  
4. Run Socinator.exe  

The launcher will dynamically load all network modules.

---

## 📚 Contribution Guidelines

- Core updates → dominatorhouse-social branch  
- Network updates → Only within their own branch  
- No cross-branch merging  
- Use Pull Requests for fixes and enhancements  

---

## 📄 License

GNU General Public License v3.0.

---

## 👥 Maintainers

- Socinator Open Source Team  
- Community Contributors  

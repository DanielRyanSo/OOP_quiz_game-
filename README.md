# OOP Game Project

This repository contains our OOP game project. Please follow the branching rules below when working on the repository.

## 🌱 Branching Rules
- **main** → stable branch (working game only). Do not push directly here.
- **dev** → development branch (all features are merged here first).
- **feature/<name>** → for individual features or fixes. Each member should create their own branch.

## 🚀 Workflow
1. Make sure you are up to date, then create a new branch for your task, commit your changes, and push:
   ```bash
   git checkout dev
   git pull
   git checkout -b feature/<your-feature>
   # Example:
   # git checkout -b feature/player-movement

   # After coding
   git add .
   git commit -m "feat: add player movement"
   git push origin feature/<your-feature>

import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
})
// This configuration file sets up Vite with React and Tailwind CSS support.
// It uses the `@vitejs/plugin-react` plugin for React support and the `@tailwindcss/vite` plugin for Tailwind CSS.
// The configuration is exported as a default export, which Vite will use to build the project.
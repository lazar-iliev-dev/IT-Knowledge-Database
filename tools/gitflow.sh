#!/usr/bin/env bash
set -euo pipefail

# --- lightweight git helper for your KnowledgeDatabase repo ---
# Usage: ./gitflow.sh <command> [args]
# Run from the repository root.

# Colors
c_red='\033[0;31m'; c_grn='\033[0;32m'; c_ylw='\033[1;33m'; c_rst='\033[0m'

echo_err() { echo -e "${c_red}[ERR]${c_rst} $*" 1>&2; }
echo_ok()  { echo -e "${c_grn}[OK]${c_rst}  $*"; }
echo_inf() { echo -e "${c_ylw}[..]${c_rst} $*"; }

require_git_repo() {
  if [ ! -d .git ]; then
    echo_err "Not a git repository. Run 'git init' first."; exit 1
  fi
}

ensure_origin() {
  if ! git remote get-url origin >/dev/null 2>&1; then
    echo_inf "No 'origin' remote configured. Set one with:\n  git remote add origin <YOUR_GITHUB_URL>"; fi
}

usage() {
  cat <<EOF
Usage: ./gitflow.sh <command> [args]

Commands:
  status                    Show short status and remotes
  start-backend             Create/switch to 'backend' branch (B)
  start-frontend            Create/switch to 'frontend' branch (B)
  feature-start <name>      Create/switch to 'feature/<name>'
  commit "message"          Stage all and commit with message
  publish                   Push current branch to origin with upstream
  sync                      Rebase current branch on top of latest main
  merge-main                Merge 'backend' and 'frontend' into main (no-ff)
  merge-current             Merge current branch into main (no-ff)
  help                      Show this help
EOF
}

cmd_status() {
  require_git_repo; ensure_origin
  echo_inf "Repository:"; git rev-parse --show-toplevel
  echo_inf "Status:"; git status -sb || true
  echo_inf "Branches:"; git branch --all --list
  echo_inf "Remotes:"; git remote -v || true
}

cmd_start_branch() {
  local name="$1"
  require_git_repo
  git checkout -B "$name"
  echo_ok "Switched to branch '$name'"
}

cmd_feature_start() {
  local name="${1:-}"
  if [ -z "$name" ]; then echo_err "feature name required"; exit 1; fi
  cmd_start_branch "feature/$name"
}

cmd_commit() {
  local msg="${1:-chore: wip}"
  require_git_repo
  git add -A
  if git diff --cached --quiet; then
    echo_inf "No staged changes to commit."
  else
    git commit -m "$msg"
    echo_ok "Committed: $msg"
  fi
}

cmd_publish() {
  require_git_repo; ensure_origin
  local cur
  cur=$(git rev-parse --abbrev-ref HEAD)
  git push -u origin "$cur"
  echo_ok "Pushed '$cur' to origin with upstream."
}

cmd_sync() {
  require_git_repo
  local cur
  cur=$(git rev-parse --abbrev-ref HEAD)
  git fetch origin || true
  if git show-ref --verify --quiet refs/heads/main; then
    git checkout main
  else
    git checkout -b main || true
  fi
  git pull --ff-only origin main || true
  git checkout "$cur"
  # prefer rebase; if it fails, fallback to merge
  if git rebase main; then
    echo_ok "Rebased '$cur' onto main."
  else
    echo_inf "Rebase failed, doing merge..."
    git rebase --abort || true
    git merge --no-ff main -m "merge: update $cur from main"
    echo_ok "Merged main into '$cur'."
  fi
}

cmd_merge_main() {
  require_git_repo
  git fetch origin || true
  if git show-ref --verify --quiet refs/heads/main; then
    git checkout main
  else
    git checkout -b main
  fi
  git pull --ff-only origin main || true
  for br in backend frontend; do
    if git show-ref --verify --quiet "refs/heads/$br"; then
      echo_inf "Merging $br into main..."
      git merge --no-ff "$br" -m "merge: $br into main"
    else
      echo_inf "Branch '$br' not found locally, skipping."
    fi
  done
  git push origin main || true
  echo_ok "main is updated."
}

cmd_merge_current() {
  require_git_repo
  local cur; cur=$(git rev-parse --abbrev-ref HEAD)
  if [ "$cur" = "main" ]; then echo_err "Already on main."; exit 1; fi
  git fetch origin || true
  git checkout main || git checkout -b main
  git pull --ff-only origin main || true
  git merge --no-ff "$cur" -m "merge: $cur into main"
  git push origin main || true
  echo_ok "Merged '$cur' into main."
}

case "${1:-help}" in
  status)          cmd_status ;;
  start-backend)   cmd_start_branch backend ;;
  start-frontend)  cmd_start_branch frontend ;;
  feature-start)   shift; cmd_feature_start "${1:-}" ;;
  commit)          shift; cmd_commit "${1:-}" ;;
  publish)         cmd_publish ;;
  sync)            cmd_sync ;;
  merge-main)      cmd_merge_main ;;
  merge-current)   cmd_merge_current ;;
  help|--help|-h)  usage ;;
  *)               usage ;;
 esac

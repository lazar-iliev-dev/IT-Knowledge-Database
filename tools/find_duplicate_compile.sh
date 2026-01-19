#!/usr/bin/env bash
set -uo pipefail

echo "=== Duplicate Compile diagnostics ==="
echo "Working dir: $(pwd)"
echo "dotnet version: $(dotnet --version || echo 'dotnet not found')"
echo

MSPP="preprocessed_KnowledgeDatabase.xml"

echo "1) Generating preprocessed MSBuild file for backend/KnowledgeDatabase.csproj (msbuild /pp)..."
if dotnet msbuild backend/KnowledgeDatabase.csproj /pp:"$MSPP"; then
  echo "-> Created $MSPP"
else
  echo "-> msbuild /pp failed (exit non-zero). Continuing with other checks."
fi
echo

echo "2) Look for the specific DTO filenames in the preprocessed output (if exists):"
files=("Dtos/ArticleDto.cs" "Dtos/CreateArticleDto.cs" "Dtos/UpdateArticleDto.cs")
if [ -f "$MSPP" ]; then
  for f in "${files[@]}"; do
    echo "---- occurrences of $f in $MSPP ----"
    grep -n -- "$f" "$MSPP" || echo " (none)"
  done
else
  echo "Preprocessed file $MSPP not found; skipping this step."
fi
echo

echo "3) Search for explicit <Compile ...> occurrences in project files (*.csproj, *.props, *.targets):"
grep -RIn --binary-files=without-match --exclude-dir=.git --include="*.csproj" --include="*.props" --include="*.targets" "<Compile" . || true
echo

echo "4) Search for explicit references to the DTO files inside project files:"
for f in "${files[@]}"; do
  echo "---- searching for $f ----"
  grep -RIn --binary-files=without-match --exclude-dir=.git --include="*.csproj" --include="*.props" --include="*.targets" "$f" . || echo " (no matches)"
done
echo

echo "5) Find any duplicates of the DTO filenames anywhere in the repository (case-insensitive):"
for name in "ArticleDto.cs" "CreateArticleDto.cs" "UpdateArticleDto.cs"; do
  echo "---- files named $name ----"
  find . -type f -iname "$name" -print || true
done
echo

echo "6) List Directory.Build.* files in repo root (common source of imports):"
ls -la Directory.Build.* 2>/dev/null || echo " (none)"
echo

echo "7) Search for EnableDefaultCompileItems (in case someone intentionally disabled implicit items):"
grep -RIn "EnableDefaultCompileItems" . || echo " (none)"
echo

echo "Diagnostics complete. What to look for:"
echo "- Any explicit <Compile Include=\"...Dtos/...\"> entries in *.csproj, *.props or *.targets -> remove them (SDK includes .cs by default)."
echo "- Any occurrence of the DTO filenames in the preprocessed output (preprocessed XML shows final imported item groups)."
echo "- Any duplicate files found by 'find' (same filename present multiple places)."
echo
echo "If you paste the relevant matches here (the lines that contain <Compile ...> or entries in the preprocessed XML), I will tell you exactly what to change."
#!/bin/sh
echo "Installing the latest installer for Arcadia..."
root=$(pwd)
wget -N https://raw.githubusercontent.com/AbnerSquared/Orikivo/tree/v1.0.0rc-1/scripts/arcadia_canary_installer.sh && bash "$root/arcadia_canary_installer.sh"
cd "$root"
rm "$root/arcadia_canary_installer.sh"
exit 0
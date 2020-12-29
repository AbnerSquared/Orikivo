root=$(pwd)
echo "Updating configuration for Arcadia."
clear
cd "$root/Orikivo/Arcadia"
mv config.json config.json.old

echo "Enter your Bot Discord Token: "
read token
echo "Enter your Discord Boats API token (Optional): "
read token_boats
echo "Enter your top.gg token (Optional): "
read token_dbl

echo "{
	\"token\": \"$token\",
	\"token_discord_boats\": \"$token_boats\",
	\"token_dbl\": \"$token_dbl\"
}" | cat - >> config.json
sleep 2
clear
cd "$root"
bash "$root/arcadia_linux_install.sh"

echo "Arcadia Installer"
root=$(pwd)
echo ""
choice=6
echo "1. Install pre-requisites"
echo "2. Download Arcadia (Canary)"
echo "3. Update configuration (Only when Arcadia is downloaded)"
echo "4. Run Arcadia"
echo "5. Exit"

while [ $choice -eq 6 ]; do
	read choice
	if [ $choice -eq 1 ]; then
		echo "Downloading pre-requisites for Arcadia. Please wait..."
		wget -N https://raw.githubusercontent.com/AbnerSquared/Orikivo/tree/v1.0.0rc-1/scripts/arcadia_install_preq.sh && bash "$root/arcadia_install_preq.sh"
		bash "$root/arcadia_linux_install.sh"
	else
		if [ $choice -eq 2 ]; then
			echo "Downloading Arcadia. Please wait..."
			wget -N https://raw.githubusercontent.com/AbnerSquared/Orikivo/tree/v1.0.0rc-1/scripts/arcadia_canary_updater.sh && bash "$root/arcadia_canary_updater.sh"
			bash "$root/arcadia_linux_install.sh"
		else
			if [ $choice -eq 3 ]; then
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
			else 
				if [ $choice -eq 4 ]; then
					echo "Now running Arcadia..."
					wget -N https://raw.githubusercontent.com/AbnerSquared/Orikivo/tree/v1.0.0rc-1/scripts/arcadia_run.sh && bash "$root/arcadia_run.sh"
					echo "Arcadia instance stopped."
					sleep 2s
					bash "$root/arcadia_linux_install.sh"
				else
					if [ $choice -eq 5 ]; then
						echo "Now closing installer."
						cd "$root"
						exit 0
					else
						clear
						echo "1. Install pre-requisites"
						echo "2. Download Arcadia (Canary)"
						echo "3. Update configuration (Only when Arcadia is downloaded)"
						echo "4. Run Arcadia"
						echo "5. Exit"
						choice=6
					fi
				fi
			fi
		fi
	fi
done

cd "$root"
exit 0
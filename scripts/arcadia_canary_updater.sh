echo "Updating Arcadia..."

if hash git 1>/dev/null 2>&1
then
	echo "Found Git."
else
	echo "Could not find Git. Please run arcadia_install_preq.sh"
	exit 1
fi

if hash dotnet 1>/dev/null 2>&1
then
	echo "Found .NET SDK."
else
	echo ".NET SDK 3.1 is missing. Please run arcadia_install_preq.sh."
	exit 1
fi

root=$(pwd)
tempd=OrikivoTemp
rm -r "$tempd" 1>/dev/null 2>&1
mkdir "$tempd"
cd "$tempd"

echo "Downloading Arcadia. Please wait..."
git clone -b v1.0.0rc-1 --recursive --depth 1 https://github.com/AbnerSquared/Orikivo
echo "Successfully downloaded Arcadia."

echo "Restoring dependencies for Arcadia..."
cd "$root/$tempd/Orikivo"
dotnet restore
echo "Successfully restored dependencies for Arcadia."

echo "Building Arcadia..."
dotnet build --configuration Release
echo "Successfully built Arcadia. Now moving files..."

cd "$root"

if [ ! -d Orikivo ]; then
	mv "$tempd"/Orikivo Orikivo
else
	rm -rf Orikivo_old 1>/dev/null 2>&1
	mv -fT Orikivo Orikivo_old 1>/dev/null 2>&1
	mv "$tempd"/Orikivo Orikivo
	cp -f "$root/Orikivo_old/Arcadia/config.json" "$root/Orikivo/Arcadia/config.json" 1>/dev/null 2>&1
	echo "Copied configuration."
	cp -RT "$root/Orikivo_old/Arcadia/bin" "$root/Orikivo/Arcadia/bin" 1>/dev/null 2>&1
	cp -RT "$root/Orikivo_old/Arcadia/bin/Release/data" "$root/Orikivo/Arcadia/bin/Release/data" 1>/dev/null 2>&1
	echo "Copied data."
	cp -RT "$root/Orikivo/assets" "$root/Orikivo/Arcadia/bin/Release/assets" 1>/dev/null 2>&1
	echo "Copied assets."

	rm -r "$tempd" 1>/dev/null 2>&1
	echo "Successfully updated Arcadia."

	cd "$root"
	rm "$root/arcadia_canary_updater.sh"
	exit 0
fi
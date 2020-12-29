echo "I have no idea what I'm doing."
root=$(pwd)

if hash dotnet 2>dev/null
then
	echo "Found .NET SDK."
else
	echo ".NET SDK 3.1 is missing. Please run arcadia_install_preq.sh."
	exit 1
fi
cd $"root/Orikivo"
dotnet restore
dotnet build -c Release
cd "$root/Orikivo/Arcadia"
echo "Starting Arcadia. Please wait..."
dotnet run -c Release
echo "Done!"

cd "$root"
rm "$root/arcadia_run.sh"
exit 0
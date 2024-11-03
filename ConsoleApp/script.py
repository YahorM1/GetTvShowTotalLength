import sys
import os
import subprocess

data = []
with open(sys.argv[1], 'r') as file:
    for line in file:
        line = line.strip()
        data.append(line)


def get_tv_show_length(show_name):
    exe_path = os.environ.get("GET_TVSHOW_TOTAL_LENGTH_BIN")
    try:
        process = subprocess.Popen(
            [exe_path, show_name],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE
        )
        stdout, stderr = process.communicate()
        if process.returncode != 0:
            return None, f"Could not get info for {show}."

        try:
            runtime = stdout.strip()
            runtime = runtime.decode("utf-8")
            runtime = int(runtime)
            return runtime, None
        except ValueError:
            return None
    except PermissionError:
        print("Permission denied")
        return None


dictionary = {}
Errors = []

for show in data:
    runtime_, error_message = get_tv_show_length(show)
    if error_message:
        Errors.append(error_message)
    else:
        dictionary[show] = runtime_

key_with_max_value = max(dictionary, key=dictionary.get)
max_value = dictionary[key_with_max_value]
max_strip = str(max_value//60)+"h "+str(max_value%60)+"m"

key_with_min_value = min(dictionary, key=dictionary.get)
min_value = dictionary[key_with_min_value]
min_strip = str(min_value//60)+"h "+str(min_value%60)+"m"

print(f"The shortest show: {key_with_min_value} ({min_strip})")
print(f"The longest show: {key_with_max_value} ({max_strip})")
if Errors:
    for error in Errors:
        print(error)

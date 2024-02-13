#!/bin/sh
echo -ne '\033c\033]0;Chat\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/chatroom_linux.x86_64" "$@"

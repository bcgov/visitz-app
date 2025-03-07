#!/bin/bash

# From a certificate's subject string, get the common name
process_string() {
    input_string="$1"

    IFS=',/' read -ra parts <<< "$input_string"
    
    # Loop through each part
    for part in "${parts[@]}"; do
        # Trim any leading or trailing spaces using sed
        trimmed_part=$(echo "$part" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')

        # Check if the part starts with "CN"
        if [[ "$trimmed_part" =~ ^CN ]]; then
            # Extract the string after the first "=" and remove any surrounding whitespace
            result=$(echo "$trimmed_part" | sed 's/^[^=]*=[[:space:]]*\(.*\)/\1/')
            echo "$result"
            return
        fi
    done

    echo "No CN found"
}

process_string "$1"

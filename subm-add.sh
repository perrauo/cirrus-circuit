# ignore submodules
ignores=$(cat <<-END
"CirrusCircuit/Assets/External/Mirror"|
"AnotherPath"|
"AnotherPath2"
END
)

# strip line feeds
ignores=$(echo $ignores| tr '\r\n' ' ')

# ignores='4|6|7'
git submodule foreach "eval \"case \$name in $ignores) ;; *) git add . ;; esac\""
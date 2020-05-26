# ignore submodules
ignores=$(cat <<-END
"AnotherPath"|
"AnotherPath2"
END
)

# strip line feeds
ignores=$(echo $ignores| tr '\r\n' ' ')

# ignores='4|6|7'
git submodule foreach "eval \"case \$name in $ignores) ;; *) git pull origin master ;; esac\""
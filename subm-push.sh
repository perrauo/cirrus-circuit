# ignore submodules
ignores=$(cat <<-END
"CirrusCircuit/Assets/External/Mirror"|
"AnotherPath"|
"AnotherPath2"
END
)

# strip line feeds
ignores=$(echo $ignores| tr '\r\n' ' ')
BASEDIR=$(dirname "$0")

# ignores='4|6|7'
git -C $BASEDIR submodule foreach "eval \"case \$name in $ignores) ;; *) git push origin HEAD:master ;; esac\""
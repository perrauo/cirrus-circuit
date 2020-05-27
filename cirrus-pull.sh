
BASEDIR=$(dirname "$0")
echo "$BASEDIR"

sh $BASEDIR/subm-pull.sh
git -C $BASEDIR pull origin master
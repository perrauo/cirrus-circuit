
BASEDIR=$(dirname "$0")
echo "$BASEDIR"

sh $BASEDIR/subm-add.sh
sh $BASEDIR/subm-commit.sh
sh $BASEDIR/subm-push.sh

git -C $BASEDIR add .
git -C $BASEDIR commit -m "upd"
git -C $BASEDIR push


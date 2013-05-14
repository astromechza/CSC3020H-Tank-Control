/*
 * Stripped down implementation of a quad tree
 * Code from https://bitbucket.org/C3/quadtree/src/a263a2865e92a7c32b6b45103ac1cd678ddd7a03/QuadTree.cs?at=default
 * Original Author: C3
 * Modified by Ben Meier to use RandomObject only
 */

using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tank_Control.Game_Objects;

namespace C3.XNA
{
    // Wrap RandomObject in an class with a QuadTreeOwner
    public class QuadTreeObject
    {
        public RandomObject Data;
        public QuadTreeNode Owner;

        public QuadTreeObject(RandomObject data)
        {
            Data = data;
        }
    }

    public class QuadTree 
    {
        private readonly Dictionary<RandomObject, QuadTreeObject> wrappedDictionary = new Dictionary<RandomObject, QuadTreeObject>();
        private readonly QuadTreeNode quadTreeRoot;


        public QuadTree(Rectangle rect)
        {
            quadTreeRoot = new QuadTreeNode(rect);
        }

        public QuadTree(int x, int y, int width, int height)
        {
            quadTreeRoot = new QuadTreeNode(new Rectangle(x, y, width, height));
        }
        
        public List<RandomObject> GetObjects(Rectangle rect)
        {
            return quadTreeRoot.GetObjects(rect);
        }

        public List<RandomObject> GetAllObjects()
        {
            return new List<RandomObject>(wrappedDictionary.Keys);
        }


        public void Add(RandomObject item)
        {
            QuadTreeObject wrappedObject = new QuadTreeObject(item);
            wrappedDictionary.Add(item, wrappedObject);
            quadTreeRoot.Insert(wrappedObject);
        }

        public void Clear()
        {
            wrappedDictionary.Clear();
            quadTreeRoot.Clear();
        }

        public bool Contains(RandomObject item)
        {
            return wrappedDictionary.ContainsKey(item);
        }

    }

    public class QuadTreeNode
    {
        private const int maxObjectsPerNode = 2;

        private List<QuadTreeObject> objects = null;
        private Rectangle rect; // The area this QuadTree represents

        private QuadTreeNode parent = null; // The parent of this quad

        private QuadTreeNode childTL = null; // Top Left Child
        private QuadTreeNode childTR = null; // Top Right Child
        private QuadTreeNode childBL = null; // Bottom Left Child
        private QuadTreeNode childBR = null; // Bottom Right Child


        public Rectangle QuadRect
        {
            get { return rect; }
        }

        public QuadTreeNode TopLeftChild
        {
            get { return childTL; }
        }

        public QuadTreeNode TopRightChild
        {
            get { return childTR; }
        }

        public QuadTreeNode BottomLeftChild
        {
            get { return childBL; }
        }

        public QuadTreeNode BottomRightChild
        {
            get { return childBR; }
        }

        public QuadTreeNode Parent
        {
            get { return parent; }
        }

        internal List<QuadTreeObject> Objects
        {
            get { return objects; }
        }

        public int Count
        {
            get { return ObjectCount(); }
        }

        public bool IsEmptyLeaf
        {
            get { return Count == 0 && childTL == null; }
        }


        public QuadTreeNode(Rectangle rect)
        {
            this.rect = rect;
        }

        public QuadTreeNode(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
        }


        private QuadTreeNode(QuadTreeNode parent, Rectangle rect) : this(rect)
        {
            this.parent = parent;
        }

        private void Add(QuadTreeObject item)
        {
            if (objects == null)
            {
                //m_objects = new List<T>();
                objects = new List<QuadTreeObject>();
            }

            item.Owner = this;
            objects.Add(item);
        }


        private void Remove(QuadTreeObject item)
        {
            if (objects != null)
            {
                int removeIndex = objects.IndexOf(item);
                if (removeIndex >= 0)
                {
                    objects[removeIndex] = objects[objects.Count - 1];
                    objects.RemoveAt(objects.Count - 1);
                }
            }
        }

        private int ObjectCount()
        {
            int count = 0;

            // Add the objects at this level
            if (objects != null)
            {
                count += objects.Count;
            }

            // Add the objects that are contained in the children
            if (childTL != null)
            {
                count += childTL.ObjectCount();
                count += childTR.ObjectCount();
                count += childBL.ObjectCount();
                count += childBR.ObjectCount();
            }

            return count;
        }


        /// <summary>
        /// Subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        private void Subdivide()
        {
            // We've reached capacity, subdivide...
            Point size = new Point(rect.Width / 2, rect.Height / 2);
            Point mid = new Point(rect.X + size.X, rect.Y + size.Y);

            childTL = new QuadTreeNode(this, new Rectangle(rect.Left, rect.Top, size.X, size.Y));
            childTR = new QuadTreeNode(this, new Rectangle(mid.X, rect.Top, size.X, size.Y));
            childBL = new QuadTreeNode(this, new Rectangle(rect.Left, mid.Y, size.X, size.Y));
            childBR = new QuadTreeNode(this, new Rectangle(mid.X, mid.Y, size.X, size.Y));

            // If they're completely contained by the quad, bump objects down
            for (int i = 0; i < objects.Count; i++)
            {
                QuadTreeNode destTree = GetDestinationTree(objects[i]);

                if (destTree != this)
                {
                    // Insert to the appropriate tree, remove the object, and back up one in the loop
                    destTree.Insert(objects[i]);
                    Remove(objects[i]);
                    i--;
                }
            }
        }


        /// <summary>
        /// Get the child Quad that would contain an object.
        /// </summary>
        /// <param name="item">The object to get a child for.</param>
        /// <returns></returns>
        private QuadTreeNode GetDestinationTree(QuadTreeObject item)
        {
            // If a child can't contain an object, it will live in this Quad
            QuadTreeNode destTree = this;

            if (childTL.QuadRect.Contains(item.Data.getRectangle()))
            {
                destTree = childTL;
            }
            else if (childTR.QuadRect.Contains(item.Data.getRectangle()))
            {
                destTree = childTR;
            }
            else if (childBL.QuadRect.Contains(item.Data.getRectangle()))
            {
                destTree = childBL;
            }
            else if (childBR.QuadRect.Contains(item.Data.getRectangle()))
            {
                destTree = childBR;
            }

            return destTree;
        }


        private void Relocate(QuadTreeObject item)
        {
            // Are we still inside our parent?
            if (QuadRect.Contains(item.Data.getRectangle()))
            {
                // Good, have we moved inside any of our children?
                if (childTL != null)
                {
                    QuadTreeNode dest = GetDestinationTree(item);
                    if (item.Owner != dest)
                    {
                        // Delete the item from this quad and add it to our child
                        // Note: Do NOT clean during this call, it can potentially delete our destination quad
                        QuadTreeNode formerOwner = item.Owner;
                        Delete(item, false);
                        dest.Insert(item);

                        // Clean up ourselves
                        formerOwner.CleanUpwards();
                    }
                }
            }
            else
            {
                // We don't fit here anymore, move up, if we can
                if (parent != null)
                {
                    parent.Relocate(item);
                }
            }
        }


        private void CleanUpwards()
        {
            if (childTL != null)
            {
                // If all the children are empty leaves, delete all the children
                if (childTL.IsEmptyLeaf &&
                    childTR.IsEmptyLeaf &&
                    childBL.IsEmptyLeaf &&
                    childBR.IsEmptyLeaf)
                {
                    childTL = null;
                    childTR = null;
                    childBL = null;
                    childBR = null;

                    if (parent != null && Count == 0)
                    {
                        parent.CleanUpwards();
                    }
                }
            }
            else
            {
                // I could be one of 4 empty leaves, tell my parent to clean up
                if (parent != null && Count == 0)
                {
                    parent.CleanUpwards();
                }
            }
        }


        #region Internal Methods

        /// <summary>
        /// Clears the QuadTree of all objects, including any objects living in its children.
        /// </summary>
        internal void Clear()
        {
            // Clear out the children, if we have any
            if (childTL != null)
            {
                childTL.Clear();
                childTR.Clear();
                childBL.Clear();
                childBR.Clear();
            }

            // Clear any objects at this level
            if (objects != null)
            {
                objects.Clear();
                objects = null;
            }

            // Set the children to null
            childTL = null;
            childTR = null;
            childBL = null;
            childBR = null;
        }


        /// <summary>
        /// Deletes an item from this QuadTree. If the object is removed causes this Quad to have no objects in its children, it's children will be removed as well.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="clean">Whether or not to clean the tree</param>
        internal void Delete(QuadTreeObject item, bool clean)
        {
            if (item.Owner != null)
            {
                if (item.Owner == this)
                {
                    Remove(item);
                    if (clean)
                    {
                        CleanUpwards();
                    }
                }
                else
                {
                    item.Owner.Delete(item, clean);
                }
            }
        }



        /// <summary>
        /// Insert an item into this QuadTree object.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        internal void Insert(QuadTreeObject item)
        {
            // If this quad doesn't contain the items rectangle, do nothing, unless we are the root
            if (!rect.Contains(item.Data.getRectangle()))
            {
                System.Diagnostics.Debug.Assert(parent == null, "We are not the root, and this object doesn't fit here. How did we get here?");
                if (parent == null)
                {
                    // This object is outside of the QuadTree bounds, we should add it at the root level
                    Add(item);
                }
                else
                {
                    return;
                }
            }

            if (objects == null ||
                (childTL == null && objects.Count + 1 <= maxObjectsPerNode))
            {
                // If there's room to add the object, just add it
                Add(item);
            }
            else
            {
                // No quads, create them and bump objects down where appropriate
                if (childTL == null)
                {
                    Subdivide();
                }

                // Find out which tree this object should go in and add it there
                QuadTreeNode destTree = GetDestinationTree(item);
                if (destTree == this)
                {
                    Add(item);
                }
                else
                {
                    destTree.Insert(item);
                }
            }
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        internal List<RandomObject> GetObjects(Rectangle searchRect)
        {
            List<RandomObject> results = new List<RandomObject>();
            GetObjects(searchRect, ref results);
            return results;
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The rectangle to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        internal void GetObjects(Rectangle searchRect, ref List<RandomObject> results)
        {
            // We can't do anything if the results list doesn't exist
            if (results != null)
            {
                if (searchRect.Contains(this.rect))
                {
                    // If the search area completely contains this quad, just get every object this quad and all it's children have
                    GetAllObjects(ref results);
                }
                else if (searchRect.Intersects(this.rect))
                {
                    // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                    if (objects != null)
                    {
                        for (int i = 0; i < objects.Count; i++)
                        {
                            if (searchRect.Intersects(objects[i].Data.getRectangle()))
                            {
                                results.Add(objects[i].Data);
                            }
                        }
                    }

                    // Get the objects for the search rectangle from the children
                    if (childTL != null)
                    {
                        childTL.GetObjects(searchRect, ref results);
                        childTR.GetObjects(searchRect, ref results);
                        childBL.GetObjects(searchRect, ref results);
                        childBR.GetObjects(searchRect, ref results);
                    }
                }
            }
        }


        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        /// <param name="results">A reference to a list in which to store the objects.</param>
        internal void GetAllObjects(ref List<RandomObject> results)
        {
            // If this Quad has objects, add them
            if (objects != null)
            {
                foreach (QuadTreeObject qto in objects)
                {
                    results.Add(qto.Data);
                }
            }

            // If we have children, get their objects too
            if (childTL != null)
            {
                childTL.GetAllObjects(ref results);
                childTR.GetAllObjects(ref results);
                childBL.GetAllObjects(ref results);
                childBR.GetAllObjects(ref results);
            }
        }


        /// <summary>
        /// Moves the QuadTree object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        internal void Move(QuadTreeObject item)
        {
            if (item.Owner != null)
            {
                item.Owner.Relocate(item);
            }
            else
            {
                Relocate(item);
            }
        }

        #endregion
    }
}